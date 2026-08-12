using FluentValidation;
using MediatR;

namespace POS.Application.Common.Validators
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Run all validators for this request asynchronously
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Gather any failures
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                // If there are failures, immediately stop the pipeline and throw
                if (failures.Count != 0)
                {
                    throw new Validators.ValidationException(failures);
                }
            }

            // If validation passes, continue to the next step in the pipeline (or the handler itself)
            return await next();
        }
    }

}