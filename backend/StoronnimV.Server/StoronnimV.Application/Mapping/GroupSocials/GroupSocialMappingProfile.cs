using AutoMapper;
using StoronnimV.Application.DTO.Responses;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Mapping.GroupSocials;

public class GroupSocialMappingProfile : Profile
{
    public GroupSocialMappingProfile()
    {
        CreateMap<GroupSocialProjection, GroupSocialResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.LinkUrl, opt => opt.MapFrom(src => src.LinkUrl));
    }
}