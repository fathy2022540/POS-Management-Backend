using POS.Domain.Entities;
using POS.Domain.Repositories;
using POS.Infrastructure.UnitOfWork;

namespace POS.Infrastructure.Persistence.POSRepositories
{
    public class InventoryRepository(IUnitOfWork<POSDBContext> unitOfWork) : IInventoryRepository
    {
        public Task<InventoryItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => unitOfWork.GetRepository<InventoryItem>().Find(id);

        public async Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            await unitOfWork.GetRepository<InventoryItem>().Update(item);
        }
    }
}
