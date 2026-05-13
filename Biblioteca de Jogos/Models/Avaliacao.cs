using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_de_Jogos.Models;

public class Avaliacao
{
    [Column("Id")]
    public int int_Id { get; set; }

    [Column("JogoId")]
    public int int_JogoId { get; set; }
    public Jogo? Jogo { get; set; }

    [Column("UsuarioNome")]
    public string str_UsuarioNome { get; set; } = string.Empty;

    [Column("Estrelas")]
    public int int_Estrelas { get; set; }

    [Column("Comentario")]
    public string? txt_Comentario { get; set; }

    [Column("Data")]
    public DateTime dat_Data { get; set; } = DateTime.UtcNow;
}