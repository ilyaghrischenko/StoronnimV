using Microsoft.AspNetCore.Antiforgery;

namespace StoronnimV.Api.Middlewares;

public sealed class AntiforgeryMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        bool isUnsafeMethod = !HttpMethods.IsGet(context.Request.Method)
            && !HttpMethods.IsHead(context.Request.Method)
            && !HttpMethods.IsOptions(context.Request.Method)
            && !HttpMethods.IsTrace(context.Request.Method);
        bool usesCookieAuthentication = context.Request.Cookies.ContainsKey("Token")
            && context.User.Identity?.IsAuthenticated == true;
        bool isLogin = context.Request.Path.Equals("/api/account/login", StringComparison.OrdinalIgnoreCase);

        if (isUnsafeMethod && (usesCookieAuthentication || isLogin))
        {
            try
            {
                await antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
        }

        await next(context);
    }
}
