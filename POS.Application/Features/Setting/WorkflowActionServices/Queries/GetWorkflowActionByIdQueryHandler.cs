using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;
using WorkflowActionEntity = POS.Domain.Entities.WorkflowAction;

namespace POS.Application.Features.WorkflowActionServices.Queries
{
    public class GetWorkflowActionByIdQuery : IRequest<ApiResponses<WorkflowActionDetailsResponse>>
    {
        public long Id { get; set; }
    }

    public class GetWorkflowActionByIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetWorkflowActionByIdQuery, ApiResponses<WorkflowActionDetailsResponse>>
    {
        public async Task<ApiResponses<WorkflowActionDetailsResponse>> Handle(
            GetWorkflowActionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var workflowActionRepo = unitOfWork.GetRepository<WorkflowActionEntity>();

            var workflowAction = await workflowActionRepo.GetFirstOrDefault<WorkflowActionEntity>(
                selector: null,
                predicate: x => x.Id == request.Id && !x.Is_Deleted,
                orderBy: null,
                include: q => q.Include(x => x.User).Include(x => x.ActionType),
                disableTracking: true);

            if (workflowAction == null)
                return ApiResponses<WorkflowActionDetailsResponse>.Failure(StatusResult.NotFound, "إجراء سير العمل غير موجود.");

            var response = AppMapper.Mapper.Map<WorkflowActionDetailsResponse>(workflowAction);

            return ApiResponses<WorkflowActionDetailsResponse>.Success(response, "تم جلب إجراء سير العمل بنجاح.");
        }
    }
}
