using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Yojigen.AegisAuth.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log the exception here (using ILogger is recommended)

            // this is where we map our custom application exceptions to HTTP status codes.
            var (statusCode, title) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
                InvalidOperationException => (HttpStatusCode.Conflict, "Conflict"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            httpContext.Response.StatusCode = (int)statusCode;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = exception.Message
            }, cancellationToken);

            return true;
        }
    }
}
