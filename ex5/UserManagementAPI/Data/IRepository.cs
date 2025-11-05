using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserManagementAPI.Data
{
    /// <summary>
    /// Generic repository interface for data access abstraction
    /// Provides common CRUD operations for entities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type T
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Retrieves an entity by its identifier
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Finds entities matching a predicate
        /// </summary>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Checks if any entity matches the predicate
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
