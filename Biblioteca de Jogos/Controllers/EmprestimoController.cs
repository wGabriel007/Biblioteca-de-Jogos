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

            ViewBag.PedidosRecebidos = pedidosRecebidos;
            ViewBag.MinhasRespostas = minhasRespostas;

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
    }
}