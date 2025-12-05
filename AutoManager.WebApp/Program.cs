using AutoManager.Apresentacao.DependencyInjection;

namespace AutoManager.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilogConfig();
        builder.Services.AddControllersWithViews();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddIdentityConfig();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Autenticacao}/{action=Login}/{id?}");

        app.Run();
    }
}
