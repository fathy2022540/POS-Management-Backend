using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using WorkflowInstanceEntity = JRM.Domain.Entities.WorkflowInstance;

namespace JRM.Application.Features.WorkflowInstanceServices.Quieries
{
    public class GetAllWorkflowInstancesQuery : Pagination, IRequest<ApiResponses<PaginationResponse<WorkflowInstanceListResponse>>>
    {
    }

    public class GetAllWorkflowInstancesQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<GetAllWorkflowInstancesQuery, ApiResponses<PaginationResponse<WorkflowInstanceListResponse>>>
    {
        public async Task<ApiResponses<PaginationResponse<WorkflowInstanceListResponse>>> Handle(
            GetAllWorkflowInstancesQuery request,
            CancellationToken cancellationToken)
        {
            var workflowInstanceRepo = unitOfWork.GetRepository<WorkflowInstanceEntity>();

            var totalCount = await workflowInstanceRepo
                .GetByCriteriaQueryable(x => !x.Is_Deleted)
                .CountAsync(cancellationToken);

            var data = await workflowInstanceRepo.GetList(
                predicate: x => !x.Is_Deleted,
                orderBy: q => q.OrderByDescending(x => x.Id),
                include: q => q.Include(x => x.WorkflowStatus),
                disableTracking: true,
                skip: (request.PageNumber - 1) * request.PageSize,
                take: request.PageSize);

            var response = new PaginationResponse<WorkflowInstanceListResponse>
            {
                Count = totalCount,
                Data = AppMapper.Mapper.Map<List<WorkflowInstanceListResponse>>(data)
            };

            return ApiResponses<PaginationResponse<WorkflowInstanceListResponse>>.Success(response, "تم جلب مثيلات سير العمل بنجاح.");
        }
    }
}
