using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca_de_Jogos.Data;
using Biblioteca_de_Jogos.Models;

public class JogosController : Controller
{
    private readonly ApplicationDbContext _context;

    public JogosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. LISTAR JOGOS (Index)
    public async Task<IActionResult> Index()
    {
        var jogos = await _context.Jogos.ToListAsync();
        return View(jogos);
    }

    // 1.1 EMPRÉSTIMOS DO JOGO
    public async Task<IActionResult> Emprestimos(int id)
    {
        var jogo = await _context.Jogos.FindAsync(id);
        if (jogo == null) return NotFound();

        // Último progresso registrado para este jogo
        var ultimoProgresso = await _context.ProgressoJogos
            .Where(p => p.JogoId == id)
            .OrderByDescending(p => p.DataAtualizacao)
            .FirstOrDefaultAsync();

        var emprestimos = await _context.Emprestimos
            .Where(e => e.JogoId == id)
            .OrderByDescending(e => e.DataEmprestimo)
            .ToListAsync();

        var modelos = emprestimos.Select(e => new JogoEmprestimoViewModel
        {
            EmprestimoId = e.Id,
            NameAmigo = e.NameAmigo,
            DataEmprestimo = e.DataEmprestimo,
            Porcentagem = ultimoProgresso?.Porcentagem ?? 0,
            DataDisponivel = e.DataEmprestimo.AddDays(14)
        }).ToList();

        ViewBag.JogoNome = jogo.Nome;
        return View(modelos);
    }

    // 2. TELA DE CADASTRO (Get)
    public IActionResult Create()
    {
        return View();
    }

    // 3. SALVAR NO BANCO (Post)
    [HttpPost]
    public async Task<IActionResult> Create(Jogo jogo)
    {
        if (ModelState.IsValid)
        {
            // Adiciona o objeto 'jogo' ao contexto (banco de dados)
            _context.Add(jogo);

            // Salva as alterações de fato no SQL Server
            await _context.SaveChangesAsync();

            // Após salvar, redireciona para a lista de jogos
            return RedirectToAction(nameof(Index));
        }

        // Se algo estiver errado, volta para a tela de cadastro com os dados preenchidos
        return View(jogo);
    }

    [HttpPost]
    public async Task<IActionResult> Emprestar(int id, string amigo)
    {
        var jogo = await _context.Jogos.FindAsync(id);
        if (jogo != null)
        {
            jogo.EstaEmprestado = true;
            // Aqui você poderia salvar o nome do amigo na tabela de Emprestimos se quiser
            _context.Update(jogo);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
