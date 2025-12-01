using Serilog;

namespace AutoManager.Apresentacao.DependencyInjection
{
    public static class SerilogConfig
    {
        public static IHostBuilder UseSerilogConfig(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)   // lê configurações do appsettings.json
                    .ReadFrom.Services(services)                     // integra com DI
                    .Enrich.FromLogContext()                         // adiciona contexto (ex: tenantId)
                    .WriteTo.Console()                               // saída no console
                    .WriteTo.File("logs/automanager-.log", rollingInterval: RollingInterval.Day); // arquivo diário
            });

            return hostBuilder;
        }
    }
}
