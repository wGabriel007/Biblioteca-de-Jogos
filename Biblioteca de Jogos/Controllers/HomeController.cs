using Biblioteca_de_Jogos.Models;
using Biblioteca_de_Jogos.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Biblioteca_de_Jogos.Services;

namespace Biblioteca_de_Jogos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              IEmailService emailService)
        {
            _logger       = logger;
            _context      = context;
            _emailService = emailService;
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

                var emailJaExiste = await _context.Usuarios
                    .AnyAsync(u => u.str_Email == user.str_Email);

                if (emailJaExiste)
                {
                    ModelState.AddModelError("Email", "Este Email já está em uso.");
                    return View(user);
                }

                var nomeJaExiste = await _context.Usuarios
                    .AnyAsync(u => u.str_Nome == user.str_Nome);

                if (nomeJaExiste)
                {
                    ModelState.AddModelError("Nome", "Este nome de usuário já está em uso.");
                    return View(user);
                }

                // Criptografa a senha antes de salvar
                user.str_Senha = BCrypt.Net.BCrypt.HashPassword(user.str_Senha);

                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();

                // Inicia a sessão automaticamente após o cadastro
                HttpContext.Session.SetString("UsuarioId",   user.int_Id.ToString());
                HttpContext.Session.SetString("UsuarioNome", user.str_Nome);
                HttpContext.Session.SetString("IsAdmin",     user.bool_Admin.ToString());
                TempData["SuccessMessage"] = $"Bem-vindo, {user.str_Nome}!";

                return RedirectToAction("Index", "Jogos");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loguin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Busca apenas pelo nome (não pela senha, pois está criptografada)
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.str_Email == model.Email);

                // Verifica se a senha informada bate com o hash salvo
                if (usuario != null && BCrypt.Net.BCrypt.Verify(model.Password, usuario.str_Senha))
                {
                    HttpContext.Session.SetString("UsuarioId",   usuario.int_Id.ToString());
                    HttpContext.Session.SetString("UsuarioNome", usuario.str_Nome);
                    HttpContext.Session.SetString("IsAdmin",     usuario.bool_Admin.ToString());
                    TempData["SuccessMessage"] = $"Bem-vindo, {usuario.str_Nome}!";

                    return RedirectToAction("Index", "Jogos");
                }

                ModelState.AddModelError("", "E-mail ou senha inválidos.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Loguin");
        }

        public IActionResult Privacy() => View();

        // GET: /Home/EsqueceuSenha
        [HttpGet]
        public IActionResult EsqueceuSenha() => View();

        // POST: /Home/EsqueceuSenha
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EsqueceuSenha(EsqueceuSenhaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.str_Email == model.Email);

            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail não encontrado.");
                return View(model);
            }

            var codigo = new Random().Next(100000, 999999).ToString();

            _context.CodigosRecuperacao.Add(new CodigoRecuperacao
            {
                txt_Email     = model.Email,
                txt_Codigo    = codigo,
                ts_Expiracao = DateTime.UtcNow.AddMinutes(15)
            });
            await _context.SaveChangesAsync();

            await _emailService.EnviarAsync(
                model.Email,
                "Código de Recuperação de Senha",
                $"<p>Seu código de recuperação é: <strong>{codigo}</strong></p>" +
                $"<p>Válido por 15 minutos.</p>");

            TempData["EmailRecuperacao"] = model.Email;
            return RedirectToAction("VerificarCodigo");
        }

        // GET: /Home/VerificarCodigo
        [HttpGet]
        public IActionResult VerificarCodigo()
        {
            var model = new VerificarCodigoViewModel
            {
                Email = TempData["EmailRecuperacao"] as string ?? ""
            };
            return View(model);
        }

        // POST: /Home/VerificarCodigo
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarCodigo(VerificarCodigoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var registro = await _context.CodigosRecuperacao
                .Where(c => c.txt_Email  == model.Email  &&
                            c.txt_Codigo == model.Codigo &&
                            !c.bool_Usado &&
                            c.ts_Expiracao > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (registro == null)
            {
                ModelState.AddModelError("", "Código inválido ou expirado.");
                return View(model);
            }

            return RedirectToAction("NovaSenha", new
            {
                email  = model.Email,
                codigo = model.Codigo
            });
        }

        // GET: /Home/NovaSenha
        [HttpGet]
        public IActionResult NovaSenha(string email, string codigo)
            => View(new NovaSenhaViewModel { Email = email, Codigo = codigo });

        // POST: /Home/NovaSenha
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NovaSenha(NovaSenhaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var registro = await _context.CodigosRecuperacao
                .FirstOrDefaultAsync(c => c.txt_Email  == model.Email  &&
                                          c.txt_Codigo == model.Codigo &&
                                          !c.bool_Usado &&
                                          c.ts_Expiracao > DateTime.UtcNow);
            if (registro == null)
            {
                ModelState.AddModelError("", "Sessão expirada. Solicite um novo código.");
                return View(model);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.str_Email == model.Email);

            if (usuario == null) return NotFound();

            usuario.str_Senha  = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha);
            registro.bool_Usado = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Senha alterada com sucesso! Faça login.";
            return RedirectToAction("Loguin");
        }

 
        // GET: /Home/EditarPerfil
        [HttpGet]
        public async Task<IActionResult> EditarPerfil()
        {
            var idStr = HttpContext.Session.GetString("UsuarioId");
            if (idStr == null) return RedirectToAction("Loguin");

            var usuario = await _context.Usuarios.FindAsync(int.Parse(idStr));
            if (usuario == null) return NotFound();

            var model = new EditarPerfilViewModel
            {
                Id    = usuario.int_Id,
                Nome  = usuario.str_Nome,
                Email = usuario.str_Email
            };

            return View(model);
        }

        // POST: /Home/EditarPerfil
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel model)
        {
            var idStr = HttpContext.Session.GetString("UsuarioId");
            if (idStr == null) return RedirectToAction("Loguin");

            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FindAsync(model.Id);
            if (usuario == null) return NotFound();

            // Verifica duplicidade de nome
            var nomeEmUso = await _context.Usuarios
                .AnyAsync(u => u.str_Nome == model.Nome && u.int_Id != model.Id);
            if (nomeEmUso)
            {
                ModelState.AddModelError("Nome", "Este nome de usuário já está em uso.");
                return View(model);
            }

            // Verifica duplicidade de e-mail
            var emailEmUso = await _context.Usuarios
                .AnyAsync(u => u.str_Email == model.Email && u.int_Id != model.Id);
            if (emailEmUso)
            {
                ModelState.AddModelError("Email", "Este e-mail já está em uso.");
                return View(model);
            }

            var nomeAntigo = usuario.str_Nome;

            usuario.str_Nome  = model.Nome;
            usuario.str_Email = model.Email;

            // Só atualiza senha se uma nova foi informada
            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
                usuario.str_Senha = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha);

            // Se o nome mudou, atualiza todas as referências nos jogos e solicitações
            if (nomeAntigo != model.Nome)
            {
                var jogosDoUsuario = await _context.Jogos
                    .Where(j => j.txt_Dono == nomeAntigo)
                    .ToListAsync();
                foreach (var j in jogosDoUsuario)
                    j.txt_Dono = model.Nome;

                var solicitacoesComoSolicitante = await _context.Solicitacoes
                    .Where(s => s.str_SolicitanteNome == nomeAntigo)
                    .ToListAsync();
                foreach (var s in solicitacoesComoSolicitante)
                    s.str_SolicitanteNome = model.Nome;

                var solicitacoesComoDono = await _context.Solicitacoes
                    .Where(s => s.str_DonoNome == nomeAntigo)
                    .ToListAsync();
                foreach (var s in solicitacoesComoDono)
                    s.str_DonoNome = model.Nome;
            }

            await _context.SaveChangesAsync();

            // Atualiza sessão
            HttpContext.Session.SetString("UsuarioNome", usuario.str_Nome);
            HttpContext.Session.SetString("IsAdmin",     usuario.bool_Admin.ToString());

            TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Index", "Jogos");
        }

        // POST: /Home/ExcluirConta
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConta()
        {
            var idStr = HttpContext.Session.GetString("UsuarioId");
            if (idStr == null) return RedirectToAction("Loguin");

            var usuario = await _context.Usuarios.FindAsync(int.Parse(idStr));
            if (usuario == null) return NotFound();

            var nome = usuario.str_Nome;

            // Remove solicitações em que o usuário é solicitante ou dono
            var solicitacoes = await _context.Solicitacoes
                .Where(s => s.str_SolicitanteNome == nome || s.str_DonoNome == nome)
                .ToListAsync();
            _context.Solicitacoes.RemoveRange(solicitacoes);

            // Remove os jogos do usuário (o CASCADE do banco já remove solicitações
            // vinculadas, mas removemos antes para garantir)
            var jogos = await _context.Jogos
                .Where(j => j.txt_Dono == nome)
                .ToListAsync();
            _context.Jogos.RemoveRange(jogos);

            // Remove códigos de recuperação do e-mail do usuário
            var codigos = await _context.CodigosRecuperacao
                .Where(c => c.txt_Email == usuario.str_Email)
                .ToListAsync();
            _context.CodigosRecuperacao.RemoveRange(codigos);

            // Remove o usuário
            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            // Encerra a sessão
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "Conta excluída com sucesso. Até logo!";
            return RedirectToAction("Loguin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
