using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper.EntitesHelper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.WorkflowActionServices.Commands.Create
{
    public record CreateWorkflowActionCommand(
        long WorkflowInstanceId,
        long UserId,
        long ActionTypeId,
        string Comments) : IRequest<ApiResponses<long>>;

    public class CreateWorkflowActionCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateWorkflowActionCommand, ApiResponses<long>>
    {
        public async Task<ApiResponses<long>> Handle(CreateWorkflowActionCommand request, CancellationToken cancellationToken)
        {
            var workflowInstance = unitOfWork.GetRepository<WorkflowInstance>();
            var userRepo = unitOfWork.GetRepository<Users>();
            var workflowActionRepo = unitOfWork.GetRepository<WorkflowAction>();

            var validation = await WorkflowActionHelper.ValidateCreateBusiness(
                workflowInstance,
                userRepo,
                request.WorkflowInstanceId,
                request.UserId);

            if (validation != null)
                return ApiResponses<long>.Failure(validation.Status, validation.Message);

            var newWorkflowAction = AppMapper.Mapper.Map<WorkflowAction>(request);

            await workflowActionRepo.Insert(newWorkflowAction);
            await unitOfWork.DoWork();

            return ApiResponses<long>.Success(newWorkflowAction.Id, "تم إنشاء إجراء سير العمل بنجاح.");
        }
    }
}
