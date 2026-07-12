using AutoMapper;
using StoronnimV.Application.DTO.Responses.NewsPage;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Mapping.News;

/// <summary>
/// Профиль маппинга для мапа с (object) в (NewsShortResponse)
/// </summary>
public class NewsShortMappingProfile : Profile
{
    public NewsShortMappingProfile()
    {
        CreateMap<NewsPaginationProjection, NewsShortResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToShortDateString()));
    }
}