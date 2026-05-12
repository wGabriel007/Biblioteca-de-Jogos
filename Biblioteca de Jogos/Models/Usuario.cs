using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("Id")]
        public int int_Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Column("Nome")]
        public string str_Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(255)]
        [Column("Senha")]
        public string str_Senha { get; set; } = string.Empty;

        [Column("IsAdmin")]
        public bool bool_Admin { get; set; } = false;

        [Required(ErrorMessage = "O email é obrigatório")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
        [Column("Email")]
        public string str_Email { get; set; } = string.Empty;
    }
}