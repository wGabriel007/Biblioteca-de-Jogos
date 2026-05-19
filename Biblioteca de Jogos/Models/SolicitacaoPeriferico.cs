using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models
{
    [Table("SolicitacoesPeriferico")]
    public class SolicitacaoPeriferico
    {
        [Key]
        [Column("Id")]
        public int int_Id { get; set; }

        [Column("PerifericoId")]
        public int int_PerifericoId { get; set; }

        [Column("SolicitanteNome")]
        public string str_SolicitanteNome { get; set; } = string.Empty;

        [Column("DonoNome")]
        public string str_DonoNome { get; set; } = string.Empty;

        // "Emprestimo" ou "Venda"
        [Column("TipoSolicitacao")]
        public string str_TipoSolicitacao { get; set; } = string.Empty;

        [Column("Status")]
        public int int_Status { get; set; }

        [Column("DataSolicitacao")]
        public DateTime ts_DataSolicitacao { get; set; }

        [Column("Visualizada")]
        public bool bool_Visualizada { get; set; }

        [ForeignKey("int_PerifericoId")]
        public Periferico? Periferico { get; set; }
    }
}
