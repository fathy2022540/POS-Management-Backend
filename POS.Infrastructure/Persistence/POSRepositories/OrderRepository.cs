using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Repositories;
using POS.Infrastructure.UnitOfWork;

namespace POS.Infrastructure.Persistence.POSRepositories
{
    public class OrderRepository(IUnitOfWork<POSDBContext> unitOfWork) : IOrderRepository
    {
        public async Task<Order?> GetByIdWithItemsAsync(long id, CancellationToken cancellationToken = default)
        {
            var repo = unitOfWork.GetRepository<Order>();
            return await repo.GetFirstOrDefault<Order>(
                selector: o => o,
                predicate: o => o.Id == id,
                orderBy: null,
                include: q => q
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payments),
                disableTracking: false);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await unitOfWork.GetRepository<Order>().Insert(order);
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await unitOfWork.GetRepository<Order>().Update(order);
        }

        public async Task<IReadOnlyList<Order>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            var orders = await unitOfWork.GetRepository<Order>().GetList(
                predicate: null,
                orderBy: q => q.OrderByDescending(o => o.CreatedDate),
                include: null,
                disableTracking: true,
                skip: skip,
                take: take);

            return orders;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => unitOfWork.GetRepository<Order>().CountAsync();

        public async Task<decimal> GetDailySalesTotalAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var query = unitOfWork.GetRepository<Order>().GetByCriteriaQueryable(
                o => o.CreatedDate >= startOfDay
                     && o.CreatedDate <= endOfDay
                     && o.Status == OrderStatus.Paid);

            if (!await query.AnyAsync(cancellationToken))
                return 0;

            return await query.SumAsync(o => o.TotalAmount, cancellationToken);
        }
    }
}
