namespace StoronnimV.Api.Contracts.Middlewares;

public interface ILoggingMiddleware
{
    public Task InvokeAsync(HttpContext context);
}