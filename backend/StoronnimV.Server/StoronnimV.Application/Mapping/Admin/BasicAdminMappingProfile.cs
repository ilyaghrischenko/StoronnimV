using AutoMapper;
using StoronnimV.Application.DTO.Responses.Admin;
using StoronnimV.Domain.Projections.Admin;

namespace StoronnimV.Application.Mapping.Admin;

public class BasicAdminMappingProfile : Profile
{
    public BasicAdminMappingProfile()
    {
        CreateMap<BasicAdminProjection, BasicAdminResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login));
    }
}