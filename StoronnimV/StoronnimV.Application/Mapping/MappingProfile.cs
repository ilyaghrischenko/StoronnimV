using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<object, NewsResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Photo")))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Title")))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Description")))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Priority")))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Date")));
    }
}