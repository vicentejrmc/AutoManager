using AutoManager.WebApp.Models;
using AutoManager.Aplicacao.DTOs;
using AutoManager.Aplicacao.ModuloAutenticacao;
using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AutenticacaoAppService autenticacaoAppService;

        public AuthController(AutenticacaoAppService autenticacaoAppService)
        {
            this.autenticacaoAppService = autenticacaoAppService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new LoginDto
            {
                UsuarioOuEmail = model.Email,
                Senha = model.Senha,
                TipoUsuario = model.TipoUsuario
            };

            var resultado = await autenticacaoAppService.LoginAsync(dto.UsuarioOuEmail, dto.Senha);

            if (resultado.Falha)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensagem);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CadastroEmpresa()
        {
            return View(new EmpresaCadastroViewModel());
        }

        [HttpPost]
        public IActionResult CadastroEmpresa(EmpresaCadastroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = autenticacaoAppService.RegistrarEmpresa(model.Usuario, model.Email, model.Senha);

            if (resultado.Falha)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensagem);
                return View(model);
            }

            return RedirectToAction("Login");
        }
    }
}
