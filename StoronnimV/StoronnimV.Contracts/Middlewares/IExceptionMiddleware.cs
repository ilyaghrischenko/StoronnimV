using System.Net;
using Microsoft.AspNetCore.Http;

namespace StoronnimV.Contracts.Middlewares;

public interface IExceptionMiddleware
{
    public Task InvokeAsync(HttpContext context);
    public Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message);
}