using AutoMapper;
using StoronnimV.Application.DTO.Responses.GroupPage.ShortMember;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Mapping.Group;

/// <summary>
/// Профиль маппинга для мапа с (object) в (SocialResponse)
/// </summary>
public class SocialMappingProfile : Profile
{
    public SocialMappingProfile()
    {
        CreateMap<SocialProjection, SocialResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SocialNetwork, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));
    }
    
}