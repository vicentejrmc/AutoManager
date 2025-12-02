using AutoManager.Aplicacao.Enums;

namespace AutoManager.Aplicacao.DTOs
{
    public class LoginDto
    {
        public string UsuarioOuEmail { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public TipoUsuarioEnum TipoUsuario { get; set; }
    }

    public class EmpresaDto
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class FuncionarioDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool EstaAtivo { get; set; }
        public Guid EmpresaId { get; set; }
    }
}
