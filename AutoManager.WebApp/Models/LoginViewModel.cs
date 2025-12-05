using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoManager.Aplicacao.Enums;

namespace AutoManager.WebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email ou usuário é obrigatório.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tipo de usuário.")]
        [Display(Name = "Tipo Usuario")]
        public TipoUsuarioEnum TipoUsuario { get; set; }
    }
}
