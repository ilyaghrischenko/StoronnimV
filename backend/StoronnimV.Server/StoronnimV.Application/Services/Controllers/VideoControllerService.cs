using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Application.DTO.Responses.Video;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Application.Services.Controllers;

public class VideoControllerService(
    IVideoService videoService,
    IMapper mapper)
    : IVideoControllerService
{
    public async Task<VideoPageResponse> GetItemByIdAsync(long id, CancellationToken ct)
    {
        VideoFullProjection video = await videoService.GetItemByIdAsync(id, ct);
        
        var videoDto = mapper.Map<VideoPageResponse>(video);
        
        return videoDto;
    }
    
    public async Task<PaginationResponse<VideoPageResponse>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args)
    {
        string type = (string)args[0];
        
        PaginationResult<VideoFullProjection> paginationResult = await videoService.GetForPageAsync(page, pageSize, ct, type);
        
        var videosDto = mapper.Map<IEnumerable<VideoPageResponse>>(paginationResult.Items);
        
        var paginationResponse = new PaginationResponse<VideoPageResponse>
        {
            CurrentPage = paginationResult.CurrentPage,
            TotalPages = paginationResult.TotalPages,
            TotalItems = paginationResult.TotalItems,
            Items = videosDto
        };
        
        return paginationResponse;
    }
}