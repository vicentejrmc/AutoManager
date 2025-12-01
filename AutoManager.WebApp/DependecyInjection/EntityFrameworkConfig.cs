using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Aplicacao.ModuloAutenticacao;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Dominio.ModuloPlanoCobranca;
using AutoManager.Dominio.ModuloPrecoCombustivel;
using AutoManager.Dominio.ModuloTaxaServico;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.ModuloAutenticacao;
using AutoManager.Infraestrutura.Orm.ModuloAutomoveis;
using AutoManager.Infraestrutura.Orm.ModuloClientes;
using AutoManager.Infraestrutura.Orm.ModuloCondutor;
using AutoManager.Infraestrutura.Orm.ModuloEmpresa;
using AutoManager.Infraestrutura.Orm.ModuloFuncionario;
using AutoManager.Infraestrutura.Orm.ModuloGrupoAutomovel;
using AutoManager.Infraestrutura.Orm.ModuloPlanoCobranca;
using AutoManager.Infraestrutura.Orm.ModuloPrecoCombustivel;
using AutoManager.Infraestrutura.Orm.ModuloTaxaServico;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Apresentacao.DependencyInjection
{
    public static class EntityFrameworkConfig
    {
        public static IServiceCollection AddEntityFrameworkConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do DbContext com PostgreSQL
            services.AddDbContext<AutoManagerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("SQL_Connection_STRING")));

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Provedor de Tenant e Autenticação
            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddScoped<AutenticacaoAppService>();
            services.AddScoped<IPasswordHasher, SenhaHasherService>();
            services.AddHttpContextAccessor();

            // Repositórios
            services.AddScoped<IRepositorioAutomovel, RepositorioAutomovelEmOrm>();
            services.AddScoped<IRepositorioCliente, RepositorioClienteEmOrm>();
            services.AddScoped<IRepositorioCondutor, RepositorioCondutorEmOrm>();
            services.AddScoped<IRepositorioEmpresa, RepositorioEmpresaEmOrm>();
            services.AddScoped<IRepositorioFuncionario, RepositorioFuncionarioEmOrm>();
            services.AddScoped<IRepositorioGrupoAutomovel, RepositorioGrupoAutomovelEmOrm>();
            services.AddScoped<IRepositorioPlanoCobranca, RepositorioPlanoCobrancaEmOrm>();
            services.AddScoped<IRepositorioPrecoCombustivel, RepositorioPrecoCombustivelEmOrm>();
            services.AddScoped<IRepositorioTaxaServico, RepositorioTaxaServicoEmOrm>();

            return services;
        }
    }
}


