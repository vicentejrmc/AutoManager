using System.ComponentModel.DataAnnotations;
using AutoManager.Aplicacao.Enums;

namespace AutoManager.WebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email ou usuário é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tipo de usuário.")]
        public TipoUsuarioEnum TipoUsuario { get; set; }
    }

    public class EmpresaCadastroViewModel
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;
    }
}
