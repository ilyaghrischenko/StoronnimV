using AutoMapper;
using StoronnimV.Application.DTO.Responses.GroupPage.ShortMember;
using StoronnimV.Domain.Projections.Member;

namespace StoronnimV.Application.Mapping.Group;

/// <summary>
/// Профиль маппинга для мапа с (object) в (MemberResponse)
/// </summary>
public class MemberMappingProfile : Profile
{
    public MemberMappingProfile()
    {
        CreateMap<MemberFullProjection, MemberResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }
}