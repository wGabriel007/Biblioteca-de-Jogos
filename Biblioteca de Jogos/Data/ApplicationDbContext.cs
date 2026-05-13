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
        public DbSet<CodigoRecuperacao> CodigosRecuperacao { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração explícita das chaves primárias
            modelBuilder.Entity<Jogo>().HasKey(j => j.int_Id);
            modelBuilder.Entity<Usuario>().HasKey(u => u.int_Id);
            modelBuilder.Entity<Console>().HasKey(c => c.int_Id);
            modelBuilder.Entity<SolicitacaoEmprestimo>().HasKey(s => s.int_Id);
            modelBuilder.Entity<CodigoRecuperacao>().HasKey(c => c.int_Id);
            modelBuilder.Entity<Avaliacao>().HasKey(a => a.int_Id);
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Jogo)
                .WithMany()
                .HasForeignKey(a => a.int_JogoId);
            modelBuilder.Entity<Avaliacao>().HasKey(a => a.int_Id);

            modelBuilder.Entity<Jogo>().ToTable("Jogos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Console>().ToTable("Consoles");
        }
    }
}
