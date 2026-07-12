namespace StoronnimV.Application.DTO.Responses.Shared;

public class PaginationResponse<T> where T : BaseResponseDto
{
    public required int CurrentPage { get; init; }
    public required int TotalPages { get; init; }
    public required int TotalItems { get; init; }
    public required IEnumerable<T> Items { get; init; }
}