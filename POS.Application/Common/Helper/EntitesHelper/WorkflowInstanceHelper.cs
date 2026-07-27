using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure.Repository;

namespace JRM.Application.Common.Helper.EntitesHelper
{
    public static class WorkflowInstanceHelper
    {
        public static async Task<ApiResponses<bool>?> ValidateCreateBusiness(
            IRepository<LookupItems> lookupRepo,
            long workflowStatusId)
        {
            if (!await lookupRepo.AnyAsync(x => x.Id == workflowStatusId && !x.Is_Deleted))
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "حالة سير العمل غير موجودة.");

            return null;
        }

        public static async Task<ApiResponses<bool>?> ValidateUpdateBusiness(
            IRepository<WorkflowInstance> workflowInstanceRepo,
            IRepository<LookupItems> lookupRepo,
            long id,
            long workflowStatusId)
        {
            if (!await workflowInstanceRepo.AnyAsync(x => x.Id == id && !x.Is_Deleted))
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "مثيل سير العمل غير موجود.");

            return await ValidateCreateBusiness(lookupRepo, workflowStatusId);
        }
    }
}
