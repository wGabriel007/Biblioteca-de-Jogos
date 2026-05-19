using Biblioteca_de_Jogos.Data;
using Biblioteca_de_Jogos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_de_Jogos.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmprestimoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? UsuarioLogado() =>
            HttpContext.Session.GetString("UsuarioNome");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(int jogoId)
        {
            var solicitante = UsuarioLogado();
            if (solicitante == null)
                return RedirectToAction("Loguin", "Home");

            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null || jogo.bool_EstaEmprestado || !jogo.bool_Disponivel || jogo.txt_Dono == solicitante)
            {
                TempData["Erro"] = "Solicitação inválida.";
                return RedirectToAction("Index", "Jogos");
            }

            var jaExiste = await _context.Solicitacoes.AnyAsync(s =>
                s.int_JogoId == jogoId &&
                s.str_SolicitanteNome == solicitante &&
                s.int_Status == (int)StatusSolicitacao.Pendente); 

            if (jaExiste)
            {
                TempData["Erro"] = "Você já tem uma solicitação pendente para este jogo.";
                return RedirectToAction("Index", "Jogos");
            }

            var solicitacao = new SolicitacaoEmprestimo
            {
                int_JogoId = jogoId,
                str_SolicitanteNome = solicitante,
                str_DonoNome = jogo.txt_Dono,
                int_Status = (int)StatusSolicitacao.Pendente, 
                ts_DataSolicitacao = DateTime.UtcNow
            };

            _context.Solicitacoes.Add(solicitacao);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Solicitação enviada para {jogo.txt_Dono}!";
            return RedirectToAction("Index", "Jogos");
        }

        public async Task<IActionResult> Notificacoes()
        {
            var nomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            if (nomeUsuario == null) return RedirectToAction("Loguin", "Home");

            // ── Jogos ──
            var pedidosRecebidos = await _context.Solicitacoes
                .Include(s => s.Jogo)
                .Where(s => s.str_DonoNome == nomeUsuario &&
                            s.int_Status == (int)StatusSolicitacao.Pendente)
                .OrderByDescending(s => s.ts_DataSolicitacao)
                .ToListAsync();

            var minhasRespostas = await _context.Solicitacoes
                .Include(s => s.Jogo)
                .Where(s => s.str_SolicitanteNome == nomeUsuario &&
                            s.int_Status != (int)StatusSolicitacao.Pendente &&
                            !s.bool_Visualizada)
                .ToListAsync();

            // ── Periféricos ──
            var pedidosPerifericosRecebidos = await _context.SolicitacoesPeriferico
                .Include(s => s.Periferico)
                .Where(s => s.str_DonoNome == nomeUsuario &&
                            s.int_Status == (int)StatusSolicitacao.Pendente)
                .OrderByDescending(s => s.ts_DataSolicitacao)
                .ToListAsync();

            var minhasRespostasPeriferico = await _context.SolicitacoesPeriferico
                .Include(s => s.Periferico)
                .Where(s => s.str_SolicitanteNome == nomeUsuario &&
                            s.int_Status != (int)StatusSolicitacao.Pendente &&
                            !s.bool_Visualizada)
                .ToListAsync();

            ViewBag.PedidosRecebidos              = pedidosRecebidos;
            ViewBag.MinhasRespostas               = minhasRespostas;
            ViewBag.PedidosPerifericosRecebidos   = pedidosPerifericosRecebidos;
            ViewBag.MinhasRespostasPeriferico      = minhasRespostasPeriferico;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aceitar(int id)
        {
            var usuario = UsuarioLogado();
            var solicitacao = await _context.Solicitacoes
                .Include(s => s.Jogo)  
                .FirstOrDefaultAsync(s => s.int_Id == id && s.str_DonoNome == usuario);

            if (solicitacao == null)
            {
                TempData["Erro"] = "Solicitação não encontrada.";
                return RedirectToAction("Notificacoes");
            }

            solicitacao.Jogo!.bool_EstaEmprestado = true;  
            solicitacao.Jogo!.str_EmprestadoPara = solicitacao.str_SolicitanteNome;  
            solicitacao.Jogo!.dt_EmprestadoEm = DateTime.UtcNow;
            solicitacao.int_Status = (int)StatusSolicitacao.Aceito;

            var outrasSolicitacoes = await _context.Solicitacoes
                .Where(s => s.int_JogoId == solicitacao.int_JogoId &&
                            s.int_Status == (int)StatusSolicitacao.Pendente &&  
                            s.int_Id != id)
                .ToListAsync();

            foreach (var outra in outrasSolicitacoes)
                outra.int_Status = (int)StatusSolicitacao.Rejeitado;  

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Empréstimo de '{solicitacao.Jogo.txt_Nome}' aceito para {solicitacao.str_SolicitanteNome}!";  // ← str_Jogo → Jogo
            return RedirectToAction("Notificacoes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rejeitar(int id)
        {
            var usuario = UsuarioLogado();
            var solicitacao = await _context.Solicitacoes
                .Include(s => s.Jogo)  
                .FirstOrDefaultAsync(s => s.int_Id == id && s.str_DonoNome == usuario);

            if (solicitacao == null)
            {
                TempData["Erro"] = "Solicitação não encontrada.";
                return RedirectToAction("Notificacoes");
            }

            solicitacao.int_Status = (int)StatusSolicitacao.Rejeitado;  
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Solicitação de '{solicitacao.Jogo!.txt_Nome}' rejeitada."; 
            return RedirectToAction("Notificacoes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int jogoId)
        {
            var usuario = UsuarioLogado();
            var jogo = await _context.Jogos.FindAsync(jogoId);

            if (jogo == null || jogo.txt_Dono != usuario)
            {
                TempData["Erro"] = "Ação inválida.";
                return RedirectToAction("Index", "Jogos");
            }

            jogo.bool_EstaEmprestado = false;
            jogo.str_EmprestadoPara = null;
            jogo.dt_EmprestadoEm = null;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{jogo.txt_Nome}' marcado como devolvido!";
            return RedirectToAction("Index", "Jogos");
        }

        [HttpGet]
        public async Task<IActionResult> MinhasSolicitacoes()
        {
            var nomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            if (nomeUsuario == null) return RedirectToAction("Loguin", "Home");

            var solicitacoes = await _context.Solicitacoes
                .Include(s => s.Jogo)
                .Where(s => s.str_SolicitanteNome == nomeUsuario &&
                            s.int_Status != (int)StatusSolicitacao.Pendente)
                .OrderByDescending(s => s.ts_DataSolicitacao)
                .ToListAsync();

            return View(solicitacoes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarVisualizada(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao != null)
            {
                solicitacao.bool_Visualizada = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Notificacoes");
        }

        // ─── PERIFÉRICOS ────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarPeriferico(int perifericoId, string tipo)
        {
            var solicitante = UsuarioLogado();
            if (solicitante == null)
                return RedirectToAction("Loguin", "Home");

            var periferico = await _context.Perifericos.FindAsync(perifericoId);
            if (periferico == null || periferico.txt_Dono == solicitante)
            {
                TempData["Erro"] = "Solicitação inválida.";
                return RedirectToAction("Index", "Jogos");
            }

            // valida disponibilidade conforme o tipo solicitado
            if (tipo == "Emprestimo" && !periferico.bool_Disponivel_Emprestimo)
            {
                TempData["Erro"] = "Este periférico não está disponível para empréstimo.";
                return RedirectToAction("Index", "Jogos");
            }
            if (tipo == "Venda" && !periferico.bool_Disponivel_Venda)
            {
                TempData["Erro"] = "Este periférico não está disponível para venda.";
                return RedirectToAction("Index", "Jogos");
            }

            var jaExiste = await _context.SolicitacoesPeriferico.AnyAsync(s =>
                s.int_PerifericoId == perifericoId &&
                s.str_SolicitanteNome == solicitante &&
                s.str_TipoSolicitacao == tipo &&
                s.int_Status == (int)StatusSolicitacao.Pendente);

            if (jaExiste)
            {
                TempData["Erro"] = $"Você já tem uma solicitação de {tipo.ToLower()} pendente para este periférico.";
                return RedirectToAction("Index", "Jogos");
            }

            _context.SolicitacoesPeriferico.Add(new SolicitacaoPeriferico
            {
                int_PerifericoId    = perifericoId,
                str_SolicitanteNome = solicitante,
                str_DonoNome        = periferico.txt_Dono,
                str_TipoSolicitacao = tipo,
                int_Status          = (int)StatusSolicitacao.Pendente,
                ts_DataSolicitacao  = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Solicitação de {tipo.ToLower()} enviada para {periferico.txt_Dono}!";
            return RedirectToAction("Index", "Jogos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarPeriferico(int id)
        {
            var usuario = UsuarioLogado();
            var solicitacao = await _context.SolicitacoesPeriferico
                .Include(s => s.Periferico)
                .FirstOrDefaultAsync(s => s.int_Id == id && s.str_DonoNome == usuario);

            if (solicitacao == null)
            {
                TempData["Erro"] = "Solicitação não encontrada.";
                return RedirectToAction("Notificacoes");
            }

            solicitacao.int_Status = (int)StatusSolicitacao.Aceito;

            // se for empréstimo, marca como indisponível para novos empréstimos
            if (solicitacao.str_TipoSolicitacao == "Emprestimo")
                solicitacao.Periferico!.bool_Disponivel_Emprestimo = false;
            // se for venda, marca como indisponível para venda
            else if (solicitacao.str_TipoSolicitacao == "Venda")
                solicitacao.Periferico!.bool_Disponivel_Venda = false;

            // rejeita outras solicitações pendentes do mesmo tipo para este periférico
            var outras = await _context.SolicitacoesPeriferico
                .Where(s => s.int_PerifericoId == solicitacao.int_PerifericoId &&
                            s.str_TipoSolicitacao == solicitacao.str_TipoSolicitacao &&
                            s.int_Status == (int)StatusSolicitacao.Pendente &&
                            s.int_Id != id)
                .ToListAsync();

            foreach (var outra in outras)
                outra.int_Status = (int)StatusSolicitacao.Rejeitado;

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Solicitação de {solicitacao.str_TipoSolicitacao.ToLower()} de '{solicitacao.Periferico!.txt_Nome}' aceita para {solicitacao.str_SolicitanteNome}!";
            return RedirectToAction("Notificacoes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarPeriferico(int id)
        {
            var usuario = UsuarioLogado();
            var solicitacao = await _context.SolicitacoesPeriferico
                .Include(s => s.Periferico)
                .FirstOrDefaultAsync(s => s.int_Id == id && s.str_DonoNome == usuario);

            if (solicitacao == null)
            {
                TempData["Erro"] = "Solicitação não encontrada.";
                return RedirectToAction("Notificacoes");
            }

            solicitacao.int_Status = (int)StatusSolicitacao.Rejeitado;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Solicitação de '{solicitacao.Periferico!.txt_Nome}' rejeitada.";
            return RedirectToAction("Notificacoes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarVisualizadaPeriferico(int id)
        {
            var solicitacao = await _context.SolicitacoesPeriferico.FindAsync(id);
            if (solicitacao != null)
            {
                solicitacao.bool_Visualizada = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Notificacoes");
        }
    }
}