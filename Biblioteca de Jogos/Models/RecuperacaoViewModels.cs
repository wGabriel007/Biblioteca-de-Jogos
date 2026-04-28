using System.ComponentModel.DataAnnotations;


namespace Biblioteca_de_Jogos.Models
{
    public class EsqueceuSenhaViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class VerificarCodigoViewModel
    {
        [Required(ErrorMessage = "Informe o código")]
        public string Codigo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class NovaSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a nova senha")]
        [StringLength(100, MinimumLength = 6, ErrorMessage ="A senha deve ter no mínimo 6 caracteres e no máximo 100.")]
        public string NovaSenha { get; set; } = string.Empty;

        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
    }
}
