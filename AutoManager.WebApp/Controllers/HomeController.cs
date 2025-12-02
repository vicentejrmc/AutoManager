using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // verificar o role do usuário logado
            // e renderizar menus diferentes para Empresa ou Funcionário
            return View();
        }
    }
}
