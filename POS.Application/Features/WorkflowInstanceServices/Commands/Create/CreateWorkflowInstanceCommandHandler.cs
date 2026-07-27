using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper.EntitesHelper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.WorkflowInstanceServices.Commands.Create
{
    public record CreateWorkflowInstanceCommand(
        string EntityType,
        long EntityId,
        string CurrentStep,
        long WorkflowStatusId) : IRequest<ApiResponses<long>>;

    public class CreateWorkflowInstanceCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<CreateWorkflowInstanceCommand, ApiResponses<long>>
    {
        public async Task<ApiResponses<long>> Handle(CreateWorkflowInstanceCommand request, CancellationToken cancellationToken)
        {
            var lookupRepo = unitOfWork.GetRepository<LookupItems>();
            var workflowInstanceRepo = unitOfWork.GetRepository<WorkflowInstance>();

            var validation = await WorkflowInstanceHelper.ValidateCreateBusiness(
                lookupRepo,
                request.WorkflowStatusId);

            if (validation != null)
                return ApiResponses<long>.Failure(validation.Status, validation.Message);

            var newWorkflowInstance = AppMapper.Mapper.Map<WorkflowInstance>(request);

            await workflowInstanceRepo.Insert(newWorkflowInstance);
            await unitOfWork.DoWork();

            return ApiResponses<long>.Success(newWorkflowInstance.Id, "تم إنشاء مثيل سير العمل بنجاح.");
        }
    }
}
