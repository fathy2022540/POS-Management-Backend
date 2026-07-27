using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.WorkflowInstanceServices.Commands.Delete
{
    public record DeleteWorkflowInstanceCommand(long Id) : IRequest<ApiResponses<bool>>;

    public class DeleteWorkflowInstanceCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<DeleteWorkflowInstanceCommand, ApiResponses<bool>>
    {
        private readonly IUnitOfWork<JRMDBContext> _unitOfWork = unitOfWork;

        public async Task<ApiResponses<bool>> Handle(DeleteWorkflowInstanceCommand request, CancellationToken cancellationToken)
        {
            var workflowInstanceRepo = _unitOfWork.GetRepository<WorkflowInstance>();

            var workflowInstance = await workflowInstanceRepo.GetFirstOrDefault<WorkflowInstance>(
                null, x => x.Id == request.Id, null, null, false);

            if (workflowInstance == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "مثيل سير العمل غير موجود.");
            }

            workflowInstance.Is_Deleted = true;

            await _unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم حذف مثيل سير العمل بنجاح.");
        }
    }
}
