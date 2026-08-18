using FluentValidation;
using POS.Application.Features.Products.Commands;

namespace POS.Application.Features.Products.Commands
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.product.Id)
                .GreaterThan(0);

            RuleFor(x => x.product.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.product.Price)
                .GreaterThanOrEqualTo(0);
        }
    }
}