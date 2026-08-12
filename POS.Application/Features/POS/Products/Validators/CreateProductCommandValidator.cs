using FluentValidation;

namespace POS.Application.Features.Products.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Product)
                .NotNull()
                .WithMessage("Product is required.");

            When(x => x.Product != null, () =>
            {
                RuleFor(x => x.Product.Code)
                    .NotEmpty()
                    .MaximumLength(50);

                RuleFor(x => x.Product.Name)
                    .NotEmpty()
                    .MaximumLength(150);

                RuleFor(x => x.Product.Price)
                    .GreaterThanOrEqualTo(0);

                RuleFor(x => x.Product)
                    .Must(product => !product.IsComposite || product.TrackInventory)
                    .WithMessage("Composite products must track inventory.");
            });
        }
    }
}