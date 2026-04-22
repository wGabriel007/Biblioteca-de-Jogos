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

        public IActionResult Loguin()
        {
            return View();
        }

        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(Usuario user)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Loguin");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Loguin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Nome == model.Username && u.Senha == model.Password);

                if(usuario != null)
                {
                    HttpContext.Session.SetString("Id", usuario.Id.ToString());
                    HttpContext.Session.SetString("Nome", usuario.Nome);
                    TempData["SucessMessage"] = $"Bem-Vindo, {usuario.Nome}";
                    return RedirectToAction("Index", "Jogos");
                }

                if (model.Username == "admin" && model.Password == "admin123")
                {
                    // Salvar informações na sessão ou criar cookie de autenticação
                    HttpContext.Session.SetString("Usuario", model.Username);

                    TempData["SuccessMessage"] = "Login realizado com sucesso!";
                    return RedirectToAction("Index", "Jogos");
                }
                else
                {
                    ModelState.AddModelError("", "Usuário ou senha inválidos.");
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Loguin");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
