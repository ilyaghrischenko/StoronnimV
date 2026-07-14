using StoronnimV.Api.Contracts.Middlewares;
using StoronnimV.Api.Models;
using StoronnimV.Application.Exceptions;

namespace StoronnimV.Api.Middlewares;

/// <summary>
/// Middleware для обработки каждой ошибки. Он позволяет отлавливать ошибку в любом месте сервера,
/// обрабатывать, и возвращать соответсвенный статус код и сообщение
/// </summary>
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
        catch (OperationCanceledException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status499ClientClosedRequest,
                ex);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status400BadRequest,
                ex);
        }
        catch (EntityNotFoundException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status404NotFound,
                ex);
        }
        catch (PaginationException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status400BadRequest,
                ex);
        }
        catch (LogInException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status401Unauthorized,
                ex);
        }
        catch (PhotoResizingException ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status415UnsupportedMediaType,
                ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context,
                StatusCodes.Status500InternalServerError,
                ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, int statusCode, Exception ex)
    {
        _logger.LogError(ex, "Request failed with status code {StatusCode}", statusCode);

        string detail = statusCode switch
        {
            StatusCodes.Status499ClientClosedRequest => "The request was cancelled.",
            StatusCodes.Status500InternalServerError => "An unexpected server error occurred.",
            _ => ex.Message
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(
            ApiErrorResponse.Create(context, statusCode, detail),
            options: null,
            contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }
}
