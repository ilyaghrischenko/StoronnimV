using AutoMapper;
using StoronnimV.Application.DTO.Responses.MusicPage;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Mapping.Music;

public class MusicPlatformMappingProfile : Profile
{
    public MusicPlatformMappingProfile()
    {
        CreateMap<MusicPlatformProjection, MusicResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BgImageUrl, opt => opt.MapFrom(src => src.BgImageUrl))
            .ForMember(dest => dest.PlatformUrl, opt => opt.MapFrom(src => src.PlatformUrl));
    }
}