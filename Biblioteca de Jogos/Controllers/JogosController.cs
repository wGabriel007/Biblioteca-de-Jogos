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
            if (UsuarioLogado() == null)
                return RedirectToAction("Loguin", "Home");

            var jogos = await _context.Jogos.ToListAsync();
            return View(jogos);
        }

        // GET: /Jogos/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (UsuarioLogado() == null)
                return RedirectToAction("Loguin", "Home");

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
