using Microsoft.EntityFrameworkCore;
using Biblioteca_de_Jogos.Models;
using Console = Biblioteca_de_Jogos.Models.Console;


namespace Biblioteca_de_Jogos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Console> Consoles { get; set; }
        public DbSet<SolicitacaoEmprestimo> Solicitacoes { get; set; }
        public DbSet<CodigoRecuperacao>  CodigosRecuperacao { get; set; }
            
    }
}
