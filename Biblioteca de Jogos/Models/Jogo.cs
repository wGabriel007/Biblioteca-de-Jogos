using System.ComponentModel.DataAnnotations;

namespace Biblioteca_de_Jogos.Models
{
    public class Jogo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O gênero é obrigatório")]
        [StringLength(100)]
        public string Genero { get; set; } = string.Empty;
        [Required(ErrorMessage = "A hora para zerar é obrigatória")]
        [StringLength(100)]
        public int HorasParaZerar { get; set; }

        [Required(ErrorMessage = "A foto é obrigatória")]
        [StringLength(100)]
        public string FotoUrl { get; set; } = string.Empty;
        public bool EstaEmprestado { get; set; }

        [Required(ErrorMessage = "O dono é obrigatório")]
        [StringLength(100)]
        public string Dono {  get; set; }
    }
}
