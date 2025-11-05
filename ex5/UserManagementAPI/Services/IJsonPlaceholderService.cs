using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Service interface for fetching initial user data from JSONPlaceholder external API
    /// </summary>
    public interface IJsonPlaceholderService
    {
        /// <summary>
        /// Fetches initial users from JSONPlaceholder API for database seeding
        /// </summary>
        /// <returns>List of users fetched from external API</returns>
        Task<List<User>> FetchInitialUsersAsync();
    }
}
