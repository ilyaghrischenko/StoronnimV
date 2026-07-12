using AutoMapper;
using StoronnimV.Application.DTO.Responses.GroupPage.ShortGroupPage;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Mapping.Group;

/// <summary>
/// Профиль маппинга для мапа с (object) в (GroupPageResponse)
/// </summary>
public class GroupPageMappingProfile : Profile
{
    public GroupPageMappingProfile()
    {
        CreateMap<GroupPageProjection, GroupPageResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}