using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Application.DTOs.Plano;
using AutoMapper;


namespace Api.Beneficiarios.Application.Mappings
{
    public class PlanoProfile : Profile
    {
        public PlanoProfile()
        {
            CreateMap<Plano, PlanoResponseDto>();
            CreateMap<CreatePlanoDto, Plano>();
            CreateMap<UpdatePlanoDto, Plano>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Nesta linha, estamos dizendo que só queremos mapear os membros que não são nulos. Isso é útil para atualizações parciais, onde você pode não querer sobrescrever valores existentes com nulos.
        }
    }
}