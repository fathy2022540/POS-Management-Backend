using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using WorkflowInstanceEntity = JRM.Domain.Entities.WorkflowInstance;

namespace JRM.Application.Features.WorkflowInstanceServices.Quieries
{
    public class GetWorkflowInstanceByIdQuery : IRequest<ApiResponses<WorkflowInstanceDetailsResponse>>
    {
        public long Id { get; set; }
    }

    public class GetWorkflowInstanceByIdQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<GetWorkflowInstanceByIdQuery, ApiResponses<WorkflowInstanceDetailsResponse>>
    {
        public async Task<ApiResponses<WorkflowInstanceDetailsResponse>> Handle(
            GetWorkflowInstanceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var workflowInstanceRepo = unitOfWork.GetRepository<WorkflowInstanceEntity>();

            var workflowInstance = await workflowInstanceRepo.GetFirstOrDefault<WorkflowInstanceEntity>(
                selector: null,
                predicate: x => x.Id == request.Id && !x.Is_Deleted,
                orderBy: null,
                include: q => q.Include(x => x.WorkflowStatus),
                disableTracking: true);

            if (workflowInstance == null)
                return ApiResponses<WorkflowInstanceDetailsResponse>.Failure(StatusResult.NotFound, "مثيل سير العمل غير موجود.");

            var response = AppMapper.Mapper.Map<WorkflowInstanceDetailsResponse>(workflowInstance);

            return ApiResponses<WorkflowInstanceDetailsResponse>.Success(response, "تم جلب مثيل سير العمل بنجاح.");
        }
    }
}
