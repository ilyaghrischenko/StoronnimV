using AutoMapper;
using StoronnimV.Application.DTO.Responses.Video;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Mapping.Video;

public class VideoPageMappingProfile : Profile
{
    public VideoPageMappingProfile()
    {
        CreateMap<VideoFullProjection, VideoPageResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}