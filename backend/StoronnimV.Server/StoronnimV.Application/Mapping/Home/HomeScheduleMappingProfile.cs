using AutoMapper;
using StoronnimV.Application.DTO.Responses.HomePage;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Application.Mapping.Home;

public class HomeScheduleMappingProfile : Profile
{
    public HomeScheduleMappingProfile()
    {
        CreateMap<ScheduleShortProjection, ScheduleHomeResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.PerformanceDateTime, opt => opt.MapFrom(src => src.PerformanceDateTime.ToShortDateString()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));
    }
}