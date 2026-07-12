namespace StoronnimV.Api.Options.RateLimiter;

public class RateLimiterOptions
{
    public required int StatusCode { get; init; }
    public required List<RateLimiterPolicy> Policies { get; init; }
}