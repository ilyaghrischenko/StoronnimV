using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IAdminControllerService
{
    
    //Delete methods
    Task DeleteNewsItemAsync(long id, CancellationToken ct);
    Task DeleteScheduleAsync(long id, CancellationToken ct);
    Task DeleteVideoAsync(long id, CancellationToken ct);
    Task DeleteGroupPageAsync(long id, CancellationToken ct);
    Task DeleteMemberAsync(long id, CancellationToken ct);
    Task DeleteMusicPlatformAsync(long id, CancellationToken ct);
    Task DeleteSocialAsync(long id, CancellationToken ct);
    Task DeleteGroupSocialAsync(long id, CancellationToken ct);
    
    //Add methods
    Task AddNewsItemAsync(NewsItemAdditionRequest request, CancellationToken ct);
    Task AddScheduleAsync(ScheduleAdditionRequest request, CancellationToken ct);
    Task AddVideoAsync(VideoAdditionRequest request, CancellationToken ct);
    Task AddGroupPageAsync(GroupPageAdditionRequest request, CancellationToken ct);
    Task AddMemberAsync(MemberAdditionRequest request, CancellationToken ct);
    Task AddMusicPlatformAsync(MusicPlatformAdditionRequest request, CancellationToken ct);
    Task AddSocialAsync(SocialAdditionRequest request, CancellationToken ct);
    Task AddGroupSocialAsync(GroupSocialAdditionRequest request, CancellationToken ct);
    
    //Update methods
    Task UpdateNewsItemAsync(NewsItemEditRequest request, CancellationToken ct);
    Task UpdateScheduleAsync(ScheduleEditRequest request, CancellationToken ct);
    Task UpdateVideoAsync(VideoEditRequest request, CancellationToken ct);
    Task UpdateGroupPageAsync(GroupPageEditRequest request, CancellationToken ct);
    Task UpdateMemberAsync(MemberEditRequest request, CancellationToken ct);
    Task UpdateMusicPlatformAsync(MusicPlatformEditRequest request, CancellationToken ct);
    Task UpdateSocialAsync(SocialEditRequest request, CancellationToken ct);
    Task UpdateGroupSocialAsync(GroupSocialEditRequest request, CancellationToken ct);
    
    //Update photo methods
    Task UpdateNewsItemPhotoAsync(PhotoEditRequest request, CancellationToken ct);
    Task DeleteNewsItemPhotoAsync(long id, CancellationToken ct);
    Task UpdateSchedulePhotoAsync(PhotoEditRequest request, CancellationToken ct);
    Task UpdateGroupPagePhotoAsync(PhotoEditRequest request, CancellationToken ct);
    Task UpdateMemberPhotoAsync(PhotoEditRequest request, CancellationToken ct);
    Task UpdateMusicPlatformPhotoAsync(PhotoEditRequest request, CancellationToken ct);
    
    //Update video methods
    Task UpdateNewsItemVideoAsync(EntityVideoEditRequest request, CancellationToken ct);
    Task DeleteNewsItemVideoAsync(long id, CancellationToken ct);
}