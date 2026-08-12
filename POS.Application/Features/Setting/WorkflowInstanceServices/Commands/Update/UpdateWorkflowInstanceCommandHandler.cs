using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper.EntitesHelper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.WorkflowInstanceServices.Commands.Update
{
    public record UpdateWorkflowInstanceCommand(
        long id,
        string EntityType,
        long EntityId,
        string CurrentStep,
        long WorkflowStatusId) : IRequest<ApiResponses<bool>>;

    public class UpdateWorkflowInstanceCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdateWorkflowInstanceCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateWorkflowInstanceCommand request, CancellationToken cancellationToken)
        {
            var workflowInstanceRepo = unitOfWork.GetRepository<WorkflowInstance>();
            var lookupRepo = unitOfWork.GetRepository<LookupItems>();

            var validation = await WorkflowInstanceHelper.ValidateUpdateBusiness(
                workflowInstanceRepo,
                lookupRepo,
                request.id,
                request.WorkflowStatusId);

            if (validation != null)
                return validation;

            var existingInstance = await workflowInstanceRepo.GetFirstOrDefault<WorkflowInstance>(
                null, x => x.Id == request.id, null, null, false);


            AppMapper.Mapper.Map(request, existingInstance!);

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم تحديث مثيل سير العمل بنجاح.");
        }
    }
}
