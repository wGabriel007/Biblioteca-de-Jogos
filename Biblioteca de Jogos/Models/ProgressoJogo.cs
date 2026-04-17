using System;

namespace Biblioteca_de_Jogos.Models
{
    public class ProgressoJogo
    {
        public int Id { get; set; }
        public int JogoId { get; set; }
        public Jogo Jogo { get; set; }
        public int Porcentagem { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
