namespace Biblioteca_de_Jogos.Models
{
    public class Jogo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int HorasParaZerar { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
        public bool EstaEmprestado { get; set; }
        public string Dono {  get; set; }
    }
}
