using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.WebApp.Models;
using AutoMapper;

namespace AutoManager.WebApp.Profiles;

public class EmpresaProfile : Profile
{
    public EmpresaProfile()
    {
        CreateMap<Empresa, EmpresaViewModel>()
            .ForMember(x => x.Senha, opt => opt.Ignore());

        CreateMap<EmpresaViewModel, Empresa>()
            .ForMember(x => x.SenhaHash, opt => opt.MapFrom(src => src.Senha ?? string.Empty))
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id ?? Guid.NewGuid()));
    }
}
