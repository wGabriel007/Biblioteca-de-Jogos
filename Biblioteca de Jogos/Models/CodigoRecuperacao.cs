using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("CodigosRecuperacao")]
    public class CodigoRecuperacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Codigo { get; set; } = string.Empty;

        public DateTime Expiracao { get; set; }

        public bool Usado { get; set; } = false;
    }
}
