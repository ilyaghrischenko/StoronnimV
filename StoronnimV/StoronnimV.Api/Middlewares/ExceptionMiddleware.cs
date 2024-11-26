using System.Net;
using StoronnimV.Contracts.Middlewares;

namespace StoronnimV.Api.Middlewares;

public class ExceptionMiddleware : IExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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

        await context.Response.WriteAsync(message);
    }
}