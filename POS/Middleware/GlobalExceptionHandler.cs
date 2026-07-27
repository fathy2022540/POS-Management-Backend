using JRM.Application.Common.Validators;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JRM.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log the error
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // 1. Handle our Custom Validation Exception (HTTP 400)
            if (exception is ValidationException validationException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                // ValidationProblemDetails is a built-in .NET class designed exactly for this
                var validationProblemDetails = new ValidationProblemDetails(validationException.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred."
                };

                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                return true; // Return true to signal that the exception has been handled
            }

            // 2. Handle generic unexpected exceptions (HTTP 500)
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred while processing your request."
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
