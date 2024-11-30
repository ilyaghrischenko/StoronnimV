using AutoMapper;
using StoronnimV.Application.Extensions;
using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Application.Mapping.Schedule;

public class ScheduleShortMappingProfile : Profile
{
    public ScheduleShortMappingProfile()
    {
        CreateMap<object, ScheduleResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (long)src.GetPropertyValue("Id")))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Photo")))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Title")))
            .ForMember(dest => dest.PerformanceDateTime, opt => opt.MapFrom(src => (string)src.GetPropertyValue("PerformanceDateTime")))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Location")))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (string)src.GetPropertyValue("Status")));
    }
}