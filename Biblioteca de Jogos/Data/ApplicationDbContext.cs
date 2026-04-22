using Microsoft.EntityFrameworkCore;
using Biblioteca_de_Jogos.Models;


namespace Biblioteca_de_Jogos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<ProgressoJogo> ProgressoJogos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
            
    }
}
