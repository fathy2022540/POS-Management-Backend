using FluentValidation;

namespace JRM.Application.Features.JourneyServices.Commands
{
    public class UpdateJourneyCommandValidator : AbstractValidator<UpdateJourneyCommand>
    {
        public UpdateJourneyCommandValidator()
        {
            // Ensure the Dto itself isn't null before validating its properties
            RuleFor(x => x.Dto).NotNull().WithMessage("Request payload cannot be null.");

            // Validate properties inside the DTO
            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.JourneyId)
                    .NotEmpty().WithMessage("Journey ID is required.");

                RuleFor(x => x.Dto.ClientEmail)
                    .NotEmpty().EmailAddress().WithMessage("A valid email is required.");

                // Example of validating a list inside the DTO
                RuleFor(x => x.Dto.SeatIds)
                    .NotEmpty().WithMessage("At least one seat must be selected.");
            });
        }
    }
}
