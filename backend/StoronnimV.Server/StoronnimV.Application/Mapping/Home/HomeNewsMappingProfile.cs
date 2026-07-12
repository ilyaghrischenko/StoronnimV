using AutoMapper;
using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Application.Mapping.Home;

public class HomeNewsMappingProfile : Profile
{
    public HomeNewsMappingProfile()
    {
        CreateMap<NewsHomeProjection, NewsHomeResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));
    }
}