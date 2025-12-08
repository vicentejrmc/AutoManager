using AutoManager.Aplicacao.ModuloAutenticacao;
using AutoManager.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers
{
    public class AutenticacaoController : Controller
    {
        private readonly AutenticacaoAppService autenticacao;

        public AutenticacaoController(AutenticacaoAppService autenticacao)
        {
            this.autenticacao = autenticacao;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await autenticacao.LoginAsync(vm.Email, vm.Senha);

            if (result.Falha)
            {
                TempData["Mensagem"] = result.Mensagem;
                TempData["Tipo"] = "danger";
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await autenticacao.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
