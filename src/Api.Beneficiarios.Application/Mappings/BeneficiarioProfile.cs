using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Domain.Entities;
using AutoMapper;

namespace Api.Beneficiarios.Application.Mappings
{
    public class BeneficiarioProfile : Profile
    {
        public BeneficiarioProfile()
        {
            CreateMap<Beneficiario, BeneficiarioResponseDto>()
                .ForMember(dest => dest.NomePlano, opt => opt.MapFrom(src => src.Plano.NomePlano)); // Mapeamento de Beneficiario para BeneficiarioResponseDto, incluindo o mapeamento do nome do plano.


            CreateMap<CreateBeneficiarioDto, Beneficiario>(); // Mapeamento de CreateBeneficiarioDto para Beneficiario

            CreateMap<UpdateBeneficiarioDto, Beneficiario>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Nesta linha, estamos dizendo que só queremos mapear os membros que não são nulos. Isso é útil para atualizações parciais, onde você pode não querer sobrescrever valores existentes com nulos.
        }
    }
}