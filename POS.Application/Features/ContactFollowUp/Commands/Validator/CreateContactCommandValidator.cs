using FluentValidation;
using JRM.Application.Features.ContactFollowUp.Commands;

namespace JRM.Application.Features.ContactFollowUp.Commands.Validator
{
    public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
                .MaximumLength(200).WithMessage("الاسم يجب ألا يتجاوز 200 حرف.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
                .Matches(@"^(\+?\d{1,4}[ -]?)?\d{7,15}$").WithMessage("رقم الهاتف غير صالح.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("يرجى تحديد سبب الطلب.")
                .MaximumLength(250).WithMessage("عنوان الموضوع يجب ألا يتجاوز 250 حرفاً.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("الرسالة مطلوبة.")
                .MinimumLength(10).WithMessage("الرسالة يجب أن تحتوي على 10 أحرف على الأقل.")
                .MaximumLength(2000).WithMessage("الرسالة لا يمكن أن تتجاوز 2000 حرف.");
        }
    }
}
