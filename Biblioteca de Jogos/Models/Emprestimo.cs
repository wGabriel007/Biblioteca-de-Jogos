namespace Biblioteca_de_Jogos.Models
{
    public class Emprestimo
    {
        public int Id { get; set; }
        public int JogoId {  get; set; }
        public Jogo Jogo {  get; set; }
        public string NameAmigo { get; set; }
        public DateTime DataEmprestimo { get; set; }
    }
}
