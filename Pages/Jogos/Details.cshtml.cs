using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Biblioteca_de_Jogos.Data;
using Biblioteca_de_Jogos.Models;

namespace Biblioteca_de_Jogos.Pages.Jogos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetailsModel(ApplicationDbContext context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Jogo? Jogo { get; set; }
        public List<ProgressoJogo> Progresso { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Jogo = await _context.Jogos.FindAsync(id);
            if (Jogo == null) return NotFound();
            Progresso = await _context.ProgressoJogos.Where(p => p.JogoId == id).OrderByDescending(p => p.DataAtualizacao).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSetProgressAsync(int id, int percentual)
        {
            var usuario = User.Identity?.Name ?? "Anônimo";
            var existing = await _context.ProgressoJogos.FirstOrDefaultAsync(p => p.JogoId == id && p.Usuario == usuario);
            if (existing != null)
            {
                existing.Percentual = Math.Clamp(percentual, 0, 100);
                existing.DataAtualizacao = DateTime.UtcNow;
                _context.Update(existing);
            }
            else
            {
                _context.ProgressoJogos.Add(new ProgressoJogo { JogoId = id, Usuario = usuario, Percentual = Math.Clamp(percentual, 0, 100) });
            }
            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostEmprestarAsync(int id, string? para)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo != null)
            {
                jogo.EstaEmprestado = !string.IsNullOrWhiteSpace(para);
                jogo.EmprestadoPara = para;
                _context.Update(jogo);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id });
        }
    }
}
