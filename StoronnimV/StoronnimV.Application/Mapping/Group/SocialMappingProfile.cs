using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.GroupPage.ShortMember;

namespace StoronnimV.Application.Mapping.Group;

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