using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Application.Mapping.GroupPage;

public class GroupPageMappingProfile : Profile
{
    
    public GroupPageMappingProfile()
    {
        CreateMap<object, GroupPageResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")!))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => (string)src.GetPropertyValue("PhotoUrl")!))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Description")!));
    }
}