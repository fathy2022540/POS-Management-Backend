using POS.Domain.Entities;

namespace POS.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdWithItemsAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetDailySalesTotalAsync(DateTime date, CancellationToken cancellationToken = default);
    }

    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdWithRecipeAsync(long id, CancellationToken cancellationToken = default);
    }

    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);
    }

    public interface IPosUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        IInventoryRepository Inventory { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
