using Microsoft.AspNetCore.WebUtilities;

namespace StoronnimV.Api.Models;

public sealed record ApiErrorResponse
{
    public required int Status { get; init; }
    public required string Title { get; init; }
    public required string Detail { get; init; }
    public required string Instance { get; init; }
    public required IReadOnlyDictionary<string, string[]> Errors { get; init; }

    public static ApiErrorResponse Create(
        HttpContext context,
        int status,
        string detail,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiErrorResponse
        {
            Status = status,
            Title = string.IsNullOrWhiteSpace(ReasonPhrases.GetReasonPhrase(status))
                ? "Request Failed"
                : ReasonPhrases.GetReasonPhrase(status),
            Detail = detail,
            Instance = context.Request.Path,
            Errors = errors ?? new Dictionary<string, string[]>()
        };
    }
}
