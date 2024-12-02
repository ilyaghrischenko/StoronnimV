using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.GroupPage.ShortMember;

namespace StoronnimV.Application.Mapping.Group;

/// <summary>
/// Профиль маппинга для мапа с (object) в (SocialResponse)
/// </summary>
public class SocialMappingProfile : Profile
{
    public SocialMappingProfile()
    {
        CreateMap<object, SocialResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")!))
            .ForMember(dest => dest.SocialNetwork, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Type")!))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Url")!));
    }
    
}