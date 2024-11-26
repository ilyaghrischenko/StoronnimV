using System.Net;
using StoronnimV.Contracts.Middlewares;

namespace StoronnimV.Api.Middlewares;

public class ExceptionMiddleware : IExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context,
                HttpStatusCode.InternalServerError,
                ex.Message);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "text/plain";
        
        _logger.LogError($"{statusCode}: {message}");
        await context.Response.WriteAsync(message);
    }
}