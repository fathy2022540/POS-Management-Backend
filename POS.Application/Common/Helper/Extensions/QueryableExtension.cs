using System.Linq.Expressions;
using JRM.Application.Common.DTOs;

namespace JRM.Application.Common.Helper.Extensions
{
    public static class QueryableExtension
    {

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, Pagination pagination)
        {
            return query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
        }

        public static IQueryable<T> BuildFilter<T>(this IQueryable<T> query, params (bool condition, Expression<Func<T, bool>> expression)[] filters)
        {
            foreach (var predicate in filters)
            {
                if (predicate.condition)
                {
                    query = query.Where(predicate.expression);
                }
            }

            return query;
        }
    }
}
