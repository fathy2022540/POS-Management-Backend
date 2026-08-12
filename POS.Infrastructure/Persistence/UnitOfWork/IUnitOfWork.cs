using Microsoft.EntityFrameworkCore;
using POS.Infrastructure.Repository;

namespace POS.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> DoWork();
        DbContext GetDbContext();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        //Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
