using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.WorkflowActionServices.Commands.Delete
{
    public record DeleteWorkflowActionCommand(long Id) : IRequest<ApiResponses<bool>>;

    public class DeleteWorkflowActionCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<DeleteWorkflowActionCommand, ApiResponses<bool>>
    {
        private readonly IUnitOfWork<POSDBContext> _unitOfWork = unitOfWork;

        public async Task<ApiResponses<bool>> Handle(DeleteWorkflowActionCommand request, CancellationToken cancellationToken)
        {
            var workflowActionRepo = _unitOfWork.GetRepository<WorkflowAction>();

            var workflowAction = await workflowActionRepo.GetFirstOrDefault<WorkflowAction>(
                null, x => x.Id == request.Id, null, null, false);

            if (workflowAction == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "إجراء سير العمل غير موجود.");
            }

            workflowAction.Is_Deleted = true;

            await _unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم حذف إجراء سير العمل بنجاح.");
        }
    }
}
