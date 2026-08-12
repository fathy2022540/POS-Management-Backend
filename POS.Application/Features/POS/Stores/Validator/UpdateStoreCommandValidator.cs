using FluentValidation;

namespace POS.Application.Features.Stores.Commands
{
    public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
    {
        public UpdateStoreCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(250);
        }
    }
}