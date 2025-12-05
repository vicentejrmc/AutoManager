using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers;

public class SharedController : Controller
{
    public IActionResult Index()
    {
        // A view Notificacao.cshtml em Views/Shared será renderizada
        return View();
    }
}
