using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure.Repository;

namespace POS.Application.Common.Helper.EntitesHelper
{
    public static class WorkflowActionHelper
    {
        public static async Task<ApiResponses<bool>?> ValidateCreateBusiness(
            IRepository<WorkflowInstance> workflowInstanceRepo,
            IRepository<Users> userRepo,
            long workflowInstanceId,
            long userId)
        {
            return await ValidateForeignKeysBusiness(workflowInstanceRepo, userRepo, workflowInstanceId, userId);
        }

        public static async Task<ApiResponses<bool>?> ValidateUpdateBusiness(
            IRepository<WorkflowAction> workflowActionRepo,
            IRepository<WorkflowInstance> workflowInstanceRepo,
            IRepository<Users> userRepo,
            long id,
            long workflowInstanceId,
            long userId)
        {
            if (!await workflowActionRepo.AnyAsync(x => x.Id == id && !x.Is_Deleted))
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "إجراء سير العمل غير موجود.");

            return await ValidateForeignKeysBusiness(workflowInstanceRepo, userRepo, workflowInstanceId, userId);
        }

        private static async Task<ApiResponses<bool>?> ValidateForeignKeysBusiness(
            IRepository<WorkflowInstance> workflowInstanceRepo,
            IRepository<Users> userRepo,
            long workflowInstanceId,
            long userId)
        {
            if (!await workflowInstanceRepo.AnyAsync(x => x.Id == workflowInstanceId && !x.Is_Deleted))
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "مثيل سير العمل غير موجود.");

            if (!await userRepo.AnyAsync(x => x.Id == userId && !x.Is_Deleted))
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "المستخدم غير موجود.");

            return null;
        }
    }
}
