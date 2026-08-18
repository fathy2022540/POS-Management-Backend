using FluentValidation;
using POS.Application.Common.DTOs.POS;
using POS.Application.Features.POS.Orders.Commands;

namespace POS.Application.Features.POS.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Items)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one order item is required.");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemDtoValidator());
        }
    }

    internal sealed class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}
