using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Application.Mapping.GroupPage;

public class MemberShortMappingProfile : Profile
{
    public MemberShortMappingProfile()
    {
        CreateMap<object, MemberShortResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")!))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => (string)src.GetPropertyValue("PhotoUrl")!))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => (string)src.GetPropertyValue("FullName")!))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Role")!));
    }
}