using System.ComponentModel.DataAnnotations;

namespace Biblioteca_de_Jogos.Models
{
    public class Jogo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do jogo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O gênero é obrigatório")]
        [StringLength(50, ErrorMessage = "O gênero deve ter no máximo 50 caracteres")]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "As horas para zerar são obrigatórias")]
        [Range(0, 9999, ErrorMessage = "Informe um valor entre 0 e 9999")]
        public int HorasParaZerar { get; set; }

        [Required(ErrorMessage = "A foto é obrigatória")]
        [StringLength(500, ErrorMessage = "A foto deve ser um URL válido para aparecer no sistema.")]
        public string? FotoUrl { get; set; }

        public bool EstaEmprestado { get; set; }

        public string? EmprestadoPara { get; set; }

        public string Dono { get; set; } = string.Empty;
    }
}
