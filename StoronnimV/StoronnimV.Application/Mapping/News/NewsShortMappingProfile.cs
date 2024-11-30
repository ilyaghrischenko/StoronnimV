using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.NewsPage;

namespace StoronnimV.Application.Mapping.News;

public class NewsShortMappingProfile : Profile
{
    public NewsShortMappingProfile()
    {
        CreateMap<object, NewsShortResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Photo")))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Title")))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Priority")))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Date")));
    }
}