using AutoManager.Aplicacao.ModuloEmpresa;
using AutoManager.WebApp.Models;
using AutoManager.Dominio.ModuloEmpresa;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AutoManager.WebApp.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly EmpresaAppService empresaService;
        private readonly IMapper mapper;

        public EmpresaController(EmpresaAppService empresaService, IMapper mapper)
        {
            this.empresaService = empresaService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Cadastrar() => View(new EmpresaViewModel());

        [HttpPost]
        public IActionResult Cadastrar(EmpresaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entidade = mapper.Map<Empresa>(vm);
            var result = empresaService.Inserir(entidade);

            TempData["Mensagem"] = result.Falha
                ? result.Mensagem
                : "Empresa registrada com sucesso.";
            TempData["Tipo"] = result.Falha ? "danger" : "success";

            return RedirectToAction("Notificacao", "Shared");
        }
    }
}
