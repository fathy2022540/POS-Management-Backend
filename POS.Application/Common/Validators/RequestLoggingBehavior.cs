using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace POS.Application.Common.Validators
{
    public class RequestLoggingBehavior<TRequest, TResponse>(
        ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation("Handling request {RequestName}", requestName);

            try
            {
                var response = await next();

                stopwatch.Stop();

                logger.LogInformation(
                    "Completed request {RequestName} in {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                logger.LogError(
                    exception,
                    "Request {RequestName} failed after {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}