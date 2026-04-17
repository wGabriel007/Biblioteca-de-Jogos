using System;

namespace Biblioteca_de_Jogos.Models
{
    public class JogoEmprestimoViewModel
    {
        public int EmprestimoId { get; set; }
        public string NameAmigo { get; set; } = string.Empty;
        public DateTime DataEmprestimo { get; set; }
        public int Porcentagem { get; set; }
        public DateTime DataDisponivel { get; set; }
    }
}
