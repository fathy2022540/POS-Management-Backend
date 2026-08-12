using System.Linq.Expressions;
using POS.Domain.Entities;
using POS.Infrastructure.Repository;

namespace POS.Application.Common.Helper
{
    public static class BusinessValidator
    {

        /// <summary>
        /// Checks a single predicate expression against the repository to see if a conflict exists.
        /// </summary>
        public static async Task<T?> FindConflictAsync<T>(
            IRepository<T> repository,
            Expression<Func<T, bool>> predicate) where T : class
        {
            // Returns the first entity that matches the conflict condition, or null if it's completely unique
            return await repository.GetFirstOrDefault<T>(
                selector: null,
                predicate: predicate,
                orderBy: null,
                include: null,
                disableTracking: false
            );
        }

        /// <summary>
        /// Gets the last entity by a specific property (typically for code/ID generation).
        /// Useful for getting the last vendor code, product code, or any sequential property.
        /// </summary>
        /// <typeparam name="T">The entity type (must inherit from BaseAuditableEntity)</typeparam>
        /// <param name="repository">The repository instance</param>
        /// <param name="propertySelector">Expression to select the property to check (e.g., v => v.VendorCode)</param>
        /// <param name="predicate">Optional predicate to filter entities (e.g., only non-null codes)</param>
        /// <returns>The last entity matching the criteria, or null if none found</returns>
        public static async Task<T?> GetLastByPropertyAsync<T>(
            IRepository<T> repository,
            Expression<Func<T, string>> propertySelector,
            Expression<Func<T, bool>>? predicate = null) where T : BaseAuditableEntity
        {
            var result = await repository.GetList(
                predicate: predicate ?? (e => true),
                orderBy: q => q.OrderByDescending(e => e.CreatedDate),
                include: null,
                disableTracking: true,
                take: 1
            );

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets the last entity by creation date with optional filtering.
        /// </summary>
        /// <typeparam name="T">The entity type (must inherit from BaseAuditableEntity)</typeparam>
        /// <param name="repository">The repository instance</param>
        /// <param name="predicate">Optional predicate to filter entities</param>
        /// <returns>The most recently created entity, or null if none found</returns>
        public static async Task<T?> GetLastCreatedAsync<T>(
            IRepository<T> repository,
            Expression<Func<T, bool>>? predicate = null) where T : BaseAuditableEntity
        {
            var result = await repository.GetList(
                predicate: predicate ?? (e => true),
                orderBy: q => q.OrderByDescending(e => e.CreatedDate),
                include: null,
                disableTracking: true,
                take: 1
            );

            return result.FirstOrDefault();
        }
    }
}
