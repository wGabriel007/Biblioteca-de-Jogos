using System.ComponentModel.DataAnnotations;

namespace Biblioteca_de_Jogos.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Nome de Usuário")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
