using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("CodigosRecuperacao")]
    public class CodigoRecuperacao
    {
        [Key]
        [Column("Id")]
        public int int_Id { get; set; }

        [Column("Email")]
        public string txt_Email { get; set; } = string.Empty;

        [Column("Codigo")]
        public string txt_Codigo { get; set; } = string.Empty;

        [Column("Expiracao")]
        public DateTime ts_Expiracao { get; set; }

        [Column("Usado")]
        public bool bool_Usado { get; set; }
    }
}