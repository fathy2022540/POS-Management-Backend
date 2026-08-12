using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper.EntitesHelper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.WorkflowActionServices.Commands.Update
{
    public record UpdateWorkflowActionCommand(
        long id,
        long WorkflowInstanceId,
        long UserId,
        long ActionTypeId,
        string Comments) : IRequest<ApiResponses<bool>>;

    public class UpdateWorkflowActionCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdateWorkflowActionCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateWorkflowActionCommand request, CancellationToken cancellationToken)
        {
            var workflowActionRepo = unitOfWork.GetRepository<WorkflowAction>();
            var workflowInstance = unitOfWork.GetRepository<WorkflowInstance>();
            var userRepo = unitOfWork.GetRepository<Users>();

            var validation = await WorkflowActionHelper.ValidateUpdateBusiness(
                workflowActionRepo,
                workflowInstance,
                userRepo,
                request.id,
                request.WorkflowInstanceId,
                request.UserId);

            if (validation != null)
                return validation;

            var existingAction = await workflowActionRepo.GetFirstOrDefault<WorkflowAction>(
                null, x => x.Id == request.id, null, null, false);

            AppMapper.Mapper.Map(request, existingAction!);

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم تحديث إجراء سير العمل بنجاح.");
        }
    }
}
