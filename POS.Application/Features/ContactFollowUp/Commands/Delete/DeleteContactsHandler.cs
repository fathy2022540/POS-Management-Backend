using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.ContactFollowUp.Commands
{
    public record DeleteContactsCommand(long Id) : IRequest<ApiResponses<bool>>;
    public class DeleteContactsHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<DeleteContactsCommand, ApiResponses<bool>>
    {
        private readonly IUnitOfWork<JRMDBContext> unitOfWork = unitOfWork;

        public async Task<ApiResponses<bool>> Handle(DeleteContactsCommand request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<Contact>();

            var contact = await repo.GetByCriteriaQueryable(c => c.Id == request.Id).AsTracking().FirstOrDefaultAsync(cancellationToken);

            if (contact == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "عذراً، لم يتم العثور على طلب التواصل.");
            }

            repo.Delete(contact);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم حذف طلب التواصل بنجاح.");
        }
    }
}
