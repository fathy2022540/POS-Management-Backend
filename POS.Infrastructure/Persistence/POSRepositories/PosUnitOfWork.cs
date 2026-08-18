using POS.Domain.Repositories;
using POS.Infrastructure.UnitOfWork;

namespace POS.Infrastructure.Persistence.POSRepositories
{
    public class PosUnitOfWork(IUnitOfWork<POSDBContext> unitOfWork) : IPosUnitOfWork
    {
        private IOrderRepository? _orders;
        private IProductRepository? _products;
        private IInventoryRepository? _inventory;

        public IOrderRepository Orders => _orders ??= new OrderRepository(unitOfWork);
        public IProductRepository Products => _products ??= new ProductRepository(unitOfWork);
        public IInventoryRepository Inventory => _inventory ??= new InventoryRepository(unitOfWork);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => unitOfWork.DoWork();

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
            => unitOfWork.BeginTransactionAsync(cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
            => unitOfWork.CommitTransactionAsync(cancellationToken);

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
            => unitOfWork.RollbackTransactionAsync(cancellationToken);
    }
}
