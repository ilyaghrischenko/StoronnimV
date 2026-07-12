using AutoMapper;
using StoronnimV.Application.DTO.Responses.SchedulePage;
using StoronnimV.Domain.Projections.Schedule;

namespace StoronnimV.Application.Mapping.Schedule;

/// <summary>
/// Профиль маппинга для мапа с (object) в (ScheduleShortResponse)
/// </summary>
public class ScheduleShortMappingProfile : Profile
{
    public ScheduleShortMappingProfile()
    {
        CreateMap<ScheduleShortProjection, ScheduleShortResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.PerformanceDateTime, opt => opt.MapFrom(src => src.PerformanceDateTime.ToString("dd.MM.yyyy HH:mm")))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));
    }
}