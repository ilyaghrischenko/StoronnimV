using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;

namespace StoronnimV.Application.Services.Controllers;

public class AdminControllerService(
    INewsService newsService,
    IVideoService videoService,
    IGroupPageService groupPageService,
    IMemberService memberService,
    IMusicPlatformService musicPlatformService,
    ISocialService socialService,
    IGroupSocialService groupSocialService,
    IScheduleService scheduleService
) : IAdminControllerService
{
    //Delete methods
    public async Task DeleteNewsItemAsync(long id, CancellationToken ct)
    {
        await newsService.DeleteNewsItemAsync(id, ct);
    }

    public async Task DeleteScheduleAsync(long id, CancellationToken ct)
    {
        await scheduleService.DeleteScheduleAsync(id, ct);
    }

    public async Task DeleteVideoAsync(long id, CancellationToken ct)
    {
        await videoService.DeleteVideoAsync(id, ct);
    }

    public async Task DeleteGroupPageAsync(long id, CancellationToken ct)
    {
        await groupPageService.DeleteGroupPageAsync(id, ct);
    }

    public async Task DeleteMemberAsync(long id, CancellationToken ct)
    {
        await memberService.DeleteMemberAsync(id, ct);
    }

    public async Task DeleteMusicPlatformAsync(long id, CancellationToken ct)
    {
        await musicPlatformService.DeleteMusicPlatformAsync(id, ct);
    }

    public async Task DeleteSocialAsync(long id, CancellationToken ct)
    {
        await socialService.DeleteSocialAsync(id, ct);
    }

    public async Task DeleteGroupSocialAsync(long id, CancellationToken ct)
    {
        await groupSocialService.DeleteGroupSocialAsync(id, ct);
    }

    //Add methods
    public async Task AddNewsItemAsync(NewsItemAdditionRequest request, CancellationToken ct)
    {
        await newsService.AddNewsItemAsync(request, ct);
    }

    public async Task AddScheduleAsync(ScheduleAdditionRequest request, CancellationToken ct)
    {
        await scheduleService.AddScheduleAsync(request, ct);
    }

    public async Task AddVideoAsync(VideoAdditionRequest request, CancellationToken ct)
    {
        await videoService.AddVideoAsync(request, ct);
    }

    public async Task AddGroupPageAsync(GroupPageAdditionRequest request, CancellationToken ct)
    {
        await groupPageService.AddGroupPageAsync(request, ct);
    }

    public async Task AddMemberAsync(MemberAdditionRequest request, CancellationToken ct)
    {
        await memberService.AddMemberAsync(request, ct);
    }

    public async Task AddMusicPlatformAsync(MusicPlatformAdditionRequest request, CancellationToken ct)
    {
        await musicPlatformService.AddMusicPlatformAsync(request, ct);
    }

    public async Task AddSocialAsync(SocialAdditionRequest request, CancellationToken ct)
    {
        await socialService.AddSocialAsync(request, ct);
    }

    public async Task AddGroupSocialAsync(GroupSocialAdditionRequest request, CancellationToken ct)
    {
        await groupSocialService.AddGroupSocialAsync(request, ct);
    }

    //Update methods
    public async Task UpdateNewsItemAsync(NewsItemEditRequest request, CancellationToken ct)
    {
        await newsService.EditNewsItemAsync(request, ct);
    }

    public async Task UpdateScheduleAsync(ScheduleEditRequest request, CancellationToken ct)
    {
        await scheduleService.UpdateScheduleAsync(request, ct);
    }

    public async Task UpdateVideoAsync(VideoEditRequest request, CancellationToken ct)
    {
        await videoService.UpdateVideoAsync(request, ct);
    }

    public async Task UpdateGroupPageAsync(GroupPageEditRequest request, CancellationToken ct)
    {
        await groupPageService.UpdateGroupPageAsync(request, ct);
    }

    public async Task UpdateMemberAsync(MemberEditRequest request, CancellationToken ct)
    {
        await memberService.UpdateMemberAsync(request, ct);
    }

    public async Task UpdateMusicPlatformAsync(MusicPlatformEditRequest request, CancellationToken ct)
    {
        await musicPlatformService.UpdateMusicPlatformAsync(request, ct);
    }

    public async Task UpdateSocialAsync(SocialEditRequest request, CancellationToken ct)
    {
        await socialService.UpdateSocialAsync(request, ct);
    }

    public async Task UpdateGroupSocialAsync(GroupSocialEditRequest request, CancellationToken ct)
    {
        await groupSocialService.UpdateGroupSocialAsync(request, ct);
    }

    //Update photo methods
    public async Task UpdateNewsItemPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        await newsService.EditNewsItemPhotoAsync(request, ct);
    }
    
    public async Task DeleteNewsItemPhotoAsync(long id, CancellationToken ct)
    {
        await newsService.DeleteNewsItemPhotoAsync(id, ct);
    }

    public async Task UpdateSchedulePhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        await scheduleService.UpdateSchedulePhotoAsync(request, ct);
    }
    
    public async Task UpdateGroupPagePhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        await groupPageService.UpdateGroupPagePhotoAsync(request, ct);
    }

    public async Task UpdateMemberPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        await memberService.UpdateMemberPhotoAsync(request, ct);
    }

    public async Task UpdateMusicPlatformPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        await musicPlatformService.UpdateMusicPlatformPhotoAsync(request, ct);
    }
    
    //Update video methods
    public async Task UpdateNewsItemVideoAsync(EntityVideoEditRequest request, CancellationToken ct)
    {
        await newsService.EditNewsItemVideoAsync(request, ct);
    }
    
    public async Task DeleteNewsItemVideoAsync(long id, CancellationToken ct)
    {
        await newsService.DeleteNewsItemVideoAsync(id, ct);
    }
}