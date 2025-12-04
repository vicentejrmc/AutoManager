using AutoManager.WebApp.Models;
using AutoMapper;

namespace AutoManager.WebApp.Profiles;

public class AutoManagerProfiles : Profile
{
    public AutoManagerProfiles()
    {
        CreateMap<LoginViewModel, LoginViewModel>();

        //CreateMap<Funcionario, FuncionarioViewModel>().ReverseMap();

        //CreateMap<Automovel, AutomovelViewModel>().ReverseMap();

        //CreateMap<GrupoAutomovel, GrupoAutomovelViewModel>().ReverseMap();

        //CreateMap<Cliente, ClienteViewModel>().ReverseMap();

        //CreateMap<Usuario, UsuarioViewModel>().ReverseMap();

        //CreateMap<Empresa, EmpresaViewModel>().ReverseMap();
    }
}
