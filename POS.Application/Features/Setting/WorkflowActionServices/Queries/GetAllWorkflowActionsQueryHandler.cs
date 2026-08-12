using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;
using WorkflowActionEntity = POS.Domain.Entities.WorkflowAction;

namespace POS.Application.Features.WorkflowActionServices.Queries
{
    public class GetAllWorkflowActionsQuery : Pagination, IRequest<ApiResponses<PaginationResponse<WorkflowActionListResponse>>>
    {
    }

    public class GetAllWorkflowActionsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllWorkflowActionsQuery, ApiResponses<PaginationResponse<WorkflowActionListResponse>>>
    {
        public async Task<ApiResponses<PaginationResponse<WorkflowActionListResponse>>> Handle(
            GetAllWorkflowActionsQuery request,
            CancellationToken cancellationToken)
        {
            var workflowActionRepo = unitOfWork.GetRepository<WorkflowActionEntity>();

            var totalCount = await workflowActionRepo
                .GetByCriteriaQueryable(x => !x.Is_Deleted)
                .CountAsync(cancellationToken);

            var data = await workflowActionRepo.GetList(
                predicate: x => !x.Is_Deleted,
                orderBy: q => q.OrderByDescending(x => x.Id),
                include: q => q.Include(x => x.ActionType),
                disableTracking: true,
                skip: (request.PageNumber - 1) * request.PageSize,
                take: request.PageSize);

            var response = new PaginationResponse<WorkflowActionListResponse>
            {
                Count = totalCount,
                Data = AppMapper.Mapper.Map<List<WorkflowActionListResponse>>(data)
            };

            return ApiResponses<PaginationResponse<WorkflowActionListResponse>>.Success(response, "تم جلب إجراءات سير العمل بنجاح.");
        }
    }
}
