using Microsoft.AspNetCore.Authentication.Cookies;

namespace AutoManager.Apresentacao.DependencyInjection
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            // Autenticação via Cookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";          // rota de login
                    options.AccessDeniedPath = "/Account/AccessDenied"; // rota de acesso negado
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);     // tempo de expiração do cookie
                    options.SlidingExpiration = true;                   // renova cookie se usuário estiver ativo
                });

            // Autorização com validação de Tenant
            services.AddAuthorization(options =>
            {
                // Política que exige que o usuário tenha um TenantId válido
                options.AddPolicy("TenantPolicy", policy =>
                    policy.RequireClaim("EmpresaId"));

                // Política para usuários do tipo Empresa
                options.AddPolicy("EmpresaPolicy", policy =>
                    policy.RequireRole("Empresa"));

                // Política para usuários do tipo Funcionario
                options.AddPolicy("FuncionarioPolicy", policy =>
                    policy.RequireRole("Funcionario"));
            });

            return services;
        }
    }
}
