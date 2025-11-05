using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Implementation of user service handling all business logic for user operations
    /// Implements service layer pattern to separate business logic from controllers
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users from the database
        /// Uses async operations to prevent blocking
        /// </summary>
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users from database");

                var users = await _context.Users
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} users", users.Count);

                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// Returns null if user not found
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID {UserId}", id);

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user with ID {UserId}", id);
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new user in the database
        /// Validates email uniqueness before creation
        /// </summary>
        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                _logger.LogInformation("Creating new user with email {Email}", createUserDto.Email);

                // Check if email already exists
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == createUserDto.Email.ToLower());

                if (emailExists)
                {
                    _logger.LogWarning("Email {Email} already exists in database", createUserDto.Email);
                    throw new InvalidOperationException($"Email '{createUserDto.Email}' is already registered");
                }

                // Create new user entity
                var user = new User
                {
                    Name = createUserDto.Name,
                    Email = createUserDto.Email,
                    Phone = createUserDto.Phone,
                    Website = createUserDto.Website,
                    Company = createUserDto.Company,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created user with ID {UserId}", user.Id);
                return MapToDto(user);
            }
            catch (InvalidOperationException)
            {
                // Re-throw business logic exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing user's information
        /// Returns null if user not found
        /// Validates email uniqueness against other users
        /// </summary>
        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.LogInformation("Updating user with ID {UserId}", id);

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", id);
                    return null;
                }

                // Check if new email conflicts with another user
                if (user.Email.ToLower() != updateUserDto.Email.ToLower())
                {
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Email.ToLower() == updateUserDto.Email.ToLower() && u.Id != id);

                    if (emailExists)
                    {
                        _logger.LogWarning("Email {Email} already exists for another user", updateUserDto.Email);
                        throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already registered");
                    }
                }

                // Update user properties
                user.Name = updateUserDto.Name;
                user.Email = updateUserDto.Email;
                user.Phone = updateUserDto.Phone;
                user.Website = updateUserDto.Website;
                user.Company = updateUserDto.Company;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated user with ID {UserId}", id);
                return MapToDto(user);
            }
            catch (InvalidOperationException)
            {
                // Re-throw business logic exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Deletes a user from the database
        /// Returns false if user not found
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID {UserId}", id);

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted user with ID {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Searches for users matching a search term in name, email, or company fields
        /// Case-insensitive search across multiple fields
        /// </summary>
        public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching users with term '{SearchTerm}'", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Return all users if search term is empty
                    return await GetAllUsersAsync();
                }

                var lowerSearchTerm = searchTerm.ToLower();

                var users = await _context.Users
                    .Where(u =>
                        u.Name.ToLower().Contains(lowerSearchTerm) ||
                        u.Email.ToLower().Contains(lowerSearchTerm) ||
                        (u.Company != null && u.Company.ToLower().Contains(lowerSearchTerm)))
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} users matching search term '{SearchTerm}'",
                    users.Count, searchTerm);

                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching users with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Maps User entity to UserDto for external consumption
        /// Separates internal data model from external API contract
        /// </summary>
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Website = user.Website,
                Company = user.Company,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
