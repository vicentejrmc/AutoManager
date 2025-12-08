using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers;

public class SharedController : Controller
{
    public IActionResult Notificacao()
    {
        return View("Notificacao");
    }
}
