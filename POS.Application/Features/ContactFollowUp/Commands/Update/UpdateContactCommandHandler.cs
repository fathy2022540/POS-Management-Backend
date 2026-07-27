using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.ContactFollowUp.Commands
{
    public record UpdateContactCommand(long Id, bool IsRead) : IRequest<ApiResponses<bool>>;
    public class UpdateContactCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
         : IRequestHandler<UpdateContactCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<Contact>();

            var contact = await repo.GetByCriteriaQueryable(c => c.Id == request.Id).AsTracking().FirstOrDefaultAsync(cancellationToken);

            if (contact == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "عذراً، لم يتم العثور على طلب التواصل المطلوب.");
            }

            contact.IsRead = request.IsRead;

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم تحديث حالة الطلب بنجاح.");
        }
    }
}
