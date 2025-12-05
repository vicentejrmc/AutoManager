using AutoManager.Aplicacao.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoManager.WebApp.Models;

public class EmpresaViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Usuario é Obrigatório.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = " Email é obrigatótio")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatoria")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
}
