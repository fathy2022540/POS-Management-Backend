using FluentValidation;
using POS.Application.Features.POS.Orders.Commands;
using POS.Domain.Enums;

namespace POS.Application.Features.POS.Orders.Validators
{
    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0);

            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.Method)
                .IsInEnum();

            RuleFor(x => x.ReferenceNumber)
                .NotEmpty()
                .When(x => x.Method is PaymentMethod.Card or PaymentMethod.MobileWallet)
                .WithMessage("Reference number is required for electronic payments.");
        }
    }
}
