using FluentValidation;
using Pos.Application.Features.Orders.Commands;

namespace POS.Application.Features.Orders.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Items)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one order item is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0);

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.Id)
                    .GreaterThan(0);

                item.RuleFor(x => x.TotalAmount)
                    .GreaterThanOrEqualTo(0);
            });
        }
    }
}