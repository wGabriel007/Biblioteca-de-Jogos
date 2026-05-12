using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("Consoles")]
    public class Console
    {
        [Key]
        [Column("Id")]
        public int int_Id { get; set; }

        [Column("Nome")]
        public string str_Nome { get; set; } = string.Empty;

        [Column("Grupo")]
        public string str_Grupo { get; set; } = string.Empty;
    }
}