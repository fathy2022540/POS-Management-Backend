using FluentValidation;

namespace POS.Application.Features.Stores.Commands
{
    public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
    {
        public CreateStoreCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(250);
        }
    }
}