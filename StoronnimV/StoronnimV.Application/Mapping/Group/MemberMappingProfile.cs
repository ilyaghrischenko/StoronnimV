using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.GroupPage.ShortMember;

namespace StoronnimV.Application.Mapping.Group;

/// <summary>
/// Профиль маппинга для мапа с (object) в (MemberResponse)
/// </summary>
public class MemberMappingProfile : Profile
{
    public MemberMappingProfile()
    {
        CreateMap<object, MemberResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")!))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => (string)src.GetPropertyValue("PhotoUrl")!))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => (string)src.GetPropertyValue("FullName")!))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Description")!))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Role")!));
    }
}