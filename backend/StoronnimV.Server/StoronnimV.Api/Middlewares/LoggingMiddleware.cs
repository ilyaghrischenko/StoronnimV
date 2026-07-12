using StoronnimV.Api.Contracts.Middlewares;

namespace StoronnimV.Api.Middlewares;

public class LoggingMiddleware : ILoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequest request = context.Request;
        _logger.LogInformation($"STARTED method: {request.Method}, path: {request.Path}");
        
        await _next(context);
        
        HttpResponse response = context.Response;
        _logger.LogInformation($"ENDED method: {request.Method}, path: {request.Path}, with status code: {response.StatusCode}");
    }
}