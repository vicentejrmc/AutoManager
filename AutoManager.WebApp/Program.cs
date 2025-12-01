using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Aplicacao.ModuloAutenticacao;
using AutoManager.Apresentacao.DependencyInjection;

namespace AutoManager.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddIdentityConfig();
        builder.Host.UseSerilogConfig();

        builder.Services.AddScoped<AutenticacaoAppService>();
        builder.Services.AddScoped<IPasswordHasher, SenhaHasherService>();
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
