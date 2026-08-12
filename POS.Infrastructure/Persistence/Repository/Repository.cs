using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Linq.Expressions;

namespace POS.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(context));
            _dbSet = _dbContext.Set<T>();
        }



        public async Task<T> Find(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await _dbContext.Set<T>().AddAsync(entity);

            return entity;
        }
        public async Task<List<T>> InsertRange(List<T> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await _dbContext.Set<T>().AddRangeAsync(entity);
            //await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<T> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbContext.Set<T>().Update(entity);
            //await _dbContext.SaveChangesAsync();
            return entity;

        }

        public async Task<List<T>> UpdateRange(List<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            _dbContext.Set<T>().UpdateRange(entities);

            return entities;
        }
        public async Task<T> Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<List<T>> DeleteRange(List<T> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbContext.Set<T>().RemoveRange(entity);
            return entity;
        }

        public async Task<List<T>> GetList(Expression<Func<T, bool>> predicate,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
               bool disableTracking, int? skip = null, int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query).AsSplitQuery();

            if (predicate != null) query = query.Where(predicate).AsSplitQuery();

            if (orderBy != null)
                return await orderBy(query.AsSplitQuery()).ToListAsync();

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.AsSplitQuery().ToListAsync();
        }


        public async Task<List<TResult>> GetListWithSelect<TResult>(
            Expression<Func<T, TResult>> selector, 
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();
            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            // تطبيق الـ Select والتحويل إلى القائمة المطلوبة
            return await query.Select(selector).ToListAsync();
        }


        public async Task<List<TResult>> GetSelectedPropertiesList<TResult>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
             Expression<Func<T, TResult>> selector,
            bool disableTracking)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
            {
                query = orderBy(query); // Apply ordering to the query
            }

            IQueryable<TResult> ApiResponsesQuery = query.Select(selector);

            return await ApiResponsesQuery.ToListAsync();
        }

        public List<T> GetListNotAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
        bool disableTracking)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }

        public async Task<TResult> GetFirstOrDefault<TResult>(
            Expression<Func<T, TResult?>> selector = null,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query).AsSplitQuery();

            if (predicate != null)
                query = query.Where(predicate).AsSplitQuery();

            if (orderBy != null)
                query = orderBy(query.AsSplitQuery());

            if (selector != null)
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                return await query.Cast<TResult>().FirstOrDefaultAsync();
            }
        }


        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);
            return await query.CountAsync();
        }
        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }



        public IQueryable<T> GetAllQeryable()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<T> GetByCriteriaQueryable(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>();

        }


    }
}
