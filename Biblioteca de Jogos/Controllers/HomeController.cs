using Biblioteca_de_Jogos.Models;
using Biblioteca_de_Jogos.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_de_Jogos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Loguin() => View();

        [HttpGet]
        public IActionResult Cadastro() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(Usuario user)
        {
            if (ModelState.IsValid)
            {
                var nomeJaExiste = await _context.Usuarios
                    .AnyAsync(u => u.Nome == user.Nome);

                if (nomeJaExiste)
                {
                    ModelState.AddModelError("Nome", "Este nome de usuário já está em uso. Escolha outro.");
                    return View(user);
                }

                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Loguin");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loguin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Nome == model.Username && u.Senha == model.Password);

                if (usuario != null)
                {
                    HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                    HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                    HttpContext.Session.SetString("IsAdmin", usuario.IsAdmin.ToString());
                    TempData["SuccessMessage"] = $"Bem-vindo, {usuario.Nome}!";
                    return RedirectToAction("Index", "Jogos");
                }

                ModelState.AddModelError("", "Usuário ou senha inválidos.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Loguin");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
