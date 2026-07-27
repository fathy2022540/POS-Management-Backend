using FluentValidation;
using JRM.Application.Features.ContactFollowUp.Commands;

namespace JRM.Application.Features.ContactFollowUp.Commands.Validator
{
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("رقم الطلب مطلوب.")
                .GreaterThan(0).WithMessage("رقم الطلب غير صالح.");

        }
    }
}
