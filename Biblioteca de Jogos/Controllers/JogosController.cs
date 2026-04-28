using Biblioteca_de_Jogos.Data;
using Biblioteca_de_Jogos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_de_Jogos.Controllers
{
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() =>
            HttpContext.Session.GetString("IsAdmin") == "True";

        private string? UsuarioLogado() =>
            HttpContext.Session.GetString("UsuarioNome");

        private bool TemPermissao(Jogo jogo) =>
            IsAdmin() || jogo.Dono == UsuarioLogado();

        // GET: /Jogos
        public async Task<IActionResult> Index()
        {
            var nomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            if (nomeUsuario == null) return RedirectToAction("Loguin", "Home");

            var jogos = await _context.Jogos.ToListAsync();

            // Solicitações pendentes exibidas nos cards
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.Status == StatusSolicitacao.Pendente)
                .ToListAsync();

            // Pedidos recebidos nos jogos do usuário (ele é o dono)
            var pedidosRecebidos = solicitacoes
                .Where(s => s.DonoNome == nomeUsuario)
                .ToList();

            // Respostas dos pedidos feitos pelo usuário (ele é o solicitante), ainda não visualizadas
            var minhasRespostas = await _context.Solicitacoes
                .Where(s => s.SolicitanteNome == nomeUsuario &&
                            s.Status != StatusSolicitacao.Pendente &&
                            !s.Visualizada)
                .ToListAsync();

            ViewBag.Solicitacoes = solicitacoes;
            ViewBag.TotalPendentes = pedidosRecebidos.Count + minhasRespostas.Count;

            return View(jogos);
        }

        public async Task<IActionResult> MeusJogos()
        {
            var nomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            if (nomeUsuario == null) return RedirectToAction("Loguin", "Home");

            var meusJogos = await _context.Jogos
                .Where(j => j.Dono == nomeUsuario)
                .ToListAsync();

            return View(meusJogos);
        }

        public async Task<IActionResult> Comunidade()
        {
            var nomeUsuario = HttpContext.Session.GetString("UsuarioNome");
            if (nomeUsuario == null) return RedirectToAction("Loguin", "Home");

            var jogosOutros = await _context.Jogos
                .Where(j => j.Dono != nomeUsuario)
                .ToListAsync();

            return View(jogosOutros);
        }

        private async Task CarregarConsoles(string? consoleSelecionado = null)
        {
            var consoles = await _context.Consoles
                .OrderBy(c => c.Grupo)
                .ThenBy(c => c.Nome)
                .ToListAsync();

            ViewBag.Consoles = consoles;
            ViewBag.ConsoleSelecionado = consoleSelecionado;
        }

        // GET: /Jogos/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (UsuarioLogado() == null)
                return RedirectToAction("Loguin", "Home");

            await CarregarConsoles();
            return View();
        }

        // POST: /Jogos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jogo jogo)
        {
            if (ModelState.IsValid)
            {
                jogo.Dono = UsuarioLogado()!;
                _context.Jogos.Add(jogo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Jogo adicionado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            await CarregarConsoles(jogo.Console);
            return View(jogo);
        }

        // GET: /Jogos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (UsuarioLogado() == null)
                return RedirectToAction("Loguin", "Home");

            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null) return NotFound();

            if (!TemPermissao(jogo))
            {
                TempData["Erro"] = "Você não tem permissão para editar este jogo.";
                return RedirectToAction(nameof(Index));
            }

            await CarregarConsoles(jogo.Console);
            return View(jogo);
        }

        // POST: /Jogos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Jogo jogo)
        {
            if (id != jogo.Id) return NotFound();

            var jogoOriginal = await _context.Jogos.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
            if (jogoOriginal == null) return NotFound();

            if (!TemPermissao(jogoOriginal))
            {
                TempData["Erro"] = "Você não tem permissão para editar este jogo.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                jogo.Dono = jogoOriginal.Dono;
                _context.Jogos.Update(jogo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Jogo atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            await CarregarConsoles(jogo.Console);
            return View(jogo);
        }

        // POST: /Jogos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null) return RedirectToAction(nameof(Index));

            if (!TemPermissao(jogo))
            {
                TempData["Erro"] = "Você não tem permissão para remover este jogo.";
                return RedirectToAction(nameof(Index));
            }

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Jogo removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}