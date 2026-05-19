using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("Perifericos")]
    public class Periferico
    {
        [Key]
        [Column("Id")]
        public int int_Id { get; set; }

        [Required(ErrorMessage = "O nome do periferico é obrigatório")]
        [StringLength(100)]
        [Column("Nome")]
        public string txt_Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Estado é obrigatorio")]
        [StringLength(50)]
        [Column("Estado")]
        public string txt_Estado { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("FotoUrl")]
        public string? txt_FotoUrl { get; set; }

        [Column("Disponivel_Emprestimo")]
        public bool bool_Disponivel_Emprestimo { get; set; } = true;
        [Column("Disponivel_Venda")]
        public bool bool_Disponivel_Venda { get; set; } = true;

        [Column("Dono")]
        public string txt_Dono { get; set; } = string.Empty;
    }
}