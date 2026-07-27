using Microsoft.EntityFrameworkCore;
using TendersManagament.Application.Common.DTOs;
using TendersManagament.Domain.Entities;
using TendersManagament.Domain.Enums;
using TendersManagament.Infrastructure.Repository;

namespace TendersManagament.Application.Common.Helper.EntitesHelper
{
    public static class VendorPriceHelper
    {
        public static async Task<ApiResponses<bool>?> ValidateVendorPriceRules(
            IRepository<Vendors> vendorRepo,
            IRepository<ProductItems> productRepo,
            IRepository<LookupItems> lookupRepo,
            IRepository<VendorProductPriceHistory> priceRepo,
            long? vendorId = null,
            long? productId = null,
            long? currencyId = null,
            bool checkDuplicate = true,
            CancellationToken cancellationToken = default)
        {
            if (vendorId.HasValue)
            {
                if (!await vendorRepo.GetByCriteriaQueryable(v => v.Id == vendorId.Value && !v.Is_Deleted).AnyAsync(cancellationToken))
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "المورد المختار غير موجود.");
            }

            if (productId.HasValue)
            {
                if (!await productRepo.GetByCriteriaQueryable(p => p.Id == productId.Value && !p.Is_Deleted).AnyAsync(cancellationToken))
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "المنتج المختار غير موجود.");
            }

            if (currencyId.HasValue)
            {
                if (!await lookupRepo.GetByCriteriaQueryable(x => x.Id == currencyId.Value).AnyAsync(cancellationToken))
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "العملة المختارة غير موجودة.");
            }

            if (checkDuplicate && vendorId.HasValue && productId.HasValue)
            {
                if (await priceRepo.GetByCriteriaQueryable(p =>
                    p.VendorId == vendorId.Value &&
                    p.ProductId == productId.Value &&
                    p.ExpiryDate == null &&
                    !p.Is_Deleted).AnyAsync(cancellationToken))
                {
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "يوجد بالفعل سعر نشط لهذا المنتج لدى هذا المورد.");
                }
            }

            return null;
        }
    }
}
