using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace POS.Infrastructure.Repository
{
    public interface IRepository<T>
    {

        Task<T> Insert(T entity);
        Task<List<T>> InsertRange(List<T> entity);
        Task<T> Update(T entity);
        Task<List<T>> UpdateRange(List<T> entities);
        Task<T> Delete(T entity);
        Task<List<T>> DeleteRange(List<T> entity);

        Task<T> Find(object id);
        Task<List<T>> GetList(Expression<Func<T, bool>> predicate,
                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                       Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
                       bool disableTracking, int? skip = null, int? take = null);

        Task<List<TResult>> GetListWithSelect<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true,
            int? skip = null,
            int? take = null);

        Task<List<TResult>> GetSelectedPropertiesList<TResult>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
             Expression<Func<T, TResult>> selector,
            bool disableTracking);


        List<T> GetListNotAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>,
            IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>,
            IIncludableQueryable<T, object>> include = null,
        bool disableTracking = true);

        public Task<TResult> GetFirstOrDefault<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
            bool disableTracking);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        //public void SaveInclude(T entity, params string[] includedProperties);




        public IQueryable<T> GetAllQeryable();

        public IQueryable<T> GetByCriteriaQueryable(Expression<Func<T, bool>> predicate);



    }
}
