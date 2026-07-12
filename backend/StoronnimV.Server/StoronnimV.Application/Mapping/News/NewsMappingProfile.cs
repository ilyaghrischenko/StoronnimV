using AutoMapper;
using StoronnimV.Application.DTO.Responses.NewsPage;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Mapping.News;

/// <summary>
/// Профиль маппинга для мапа с (object) в (NewsResponse)
/// </summary>
public class NewsMappingProfile : Profile
{
    public NewsMappingProfile()
    {
        CreateMap<NewsFullProjection, NewsResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Video, opt => opt.MapFrom(src => src.Video))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToShortDateString()));
    }
}