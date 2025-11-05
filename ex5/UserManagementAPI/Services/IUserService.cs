using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Service interface for user-related business logic operations
    /// Provides abstraction layer between controllers and data access
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users from the system
        /// </summary>
        /// <returns>List of all users as DTOs</returns>
        Task<List<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Retrieves a specific user by their unique identifier
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User DTO if found, null otherwise</returns>
        Task<UserDto?> GetUserByIdAsync(int id);

        /// <summary>
        /// Creates a new user in the system
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user as DTO</returns>
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

        /// <summary>
        /// Updates an existing user's information
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="updateUserDto">Updated user data</param>
        /// <returns>Updated user as DTO if successful, null if user not found</returns>
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);

        /// <summary>
        /// Deletes a user from the system
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>True if deleted successfully, false if user not found</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Searches for users matching a search term in name, email, or company fields
        /// </summary>
        /// <param name="searchTerm">Search term to match against user fields</param>
        /// <returns>List of matching users as DTOs</returns>
        Task<List<UserDto>> SearchUsersAsync(string searchTerm);
    }
}
