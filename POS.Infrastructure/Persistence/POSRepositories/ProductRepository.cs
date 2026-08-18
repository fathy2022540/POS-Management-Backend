using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Repositories;
using POS.Infrastructure.UnitOfWork;

namespace POS.Infrastructure.Persistence.POSRepositories
{
    public class ProductRepository(IUnitOfWork<POSDBContext> unitOfWork) : IProductRepository
    {
        public Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => unitOfWork.GetRepository<Product>().Find(id);

        public async Task<Product?> GetByIdWithRecipeAsync(long id, CancellationToken cancellationToken = default)
        {
            return await unitOfWork.GetRepository<Product>().GetFirstOrDefault<Product>(
                selector: p => p,
                predicate: p => p.Id == id,
                orderBy: null,
                include: q => q.Include(p => p.RecipeItems),
                disableTracking: false);
        }
    }
}
