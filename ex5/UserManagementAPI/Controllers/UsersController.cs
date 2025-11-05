using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Common;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    /// <summary>
    /// RESTful API controller for user management operations
    /// Provides endpoints for CRUD operations and search functionality
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users from the system
        /// </summary>
        /// <returns>List of all users wrapped in ApiResponse</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("GET /api/users - Fetching all users");

                var users = await _userService.GetAllUsersAsync();
                return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users");
                return StatusCode(500, ApiResponse<List<UserDto>>.FailureResponse(
                    "An error occurred while retrieving users",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User if found, 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("GET /api/users/{Id} - Fetching user by ID", id);

                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.FailureResponse(
                        $"User with ID {id} not found",
                        new List<string> { "User does not exist" }));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID {Id}", id);
                return StatusCode(500, ApiResponse<UserDto>.FailureResponse(
                    "An error occurred while retrieving the user",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Searches for users matching a search term
        /// Searches across name, email, and company fields
        /// </summary>
        /// <param name="searchTerm">Search term to match</param>
        /// <returns>List of matching users</returns>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("GET /api/users/search?searchTerm={SearchTerm} - Searching users", searchTerm);

                var users = await _userService.SearchUsersAsync(searchTerm);
                return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users,
                    $"Found {users.Count} user(s) matching search term"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching users with term '{SearchTerm}'", searchTerm);
                return StatusCode(500, ApiResponse<List<UserDto>>.FailureResponse(
                    "An error occurred while searching users",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Creates a new user in the system
        /// Validates input data and ensures email uniqueness
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user with 201 status code</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                _logger.LogInformation("POST /api/users - Creating new user");

                // Model validation is handled by data annotations
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState.Values)
                    {
                        foreach (var error in state.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    return BadRequest(ApiResponse<UserDto>.FailureResponse(
                        "Validation failed",
                        errors));
                }

                // Check for duplicate email before creating user
                // Case-insensitive comparison prevents duplicate emails with different casing
                // This provides an explicit check at the controller level for better error handling
                var existingUsers = await _userService.GetAllUsersAsync();
                var emailExists = existingUsers.Any(u =>
                    u.Email.Equals(createUserDto.Email, StringComparison.OrdinalIgnoreCase));

                if (emailExists)
                {
                    _logger.LogWarning("Attempt to create user with duplicate email: {Email}", createUserDto.Email);
                    return BadRequest(ApiResponse<UserDto>.FailureResponse(
                        "Email already exists in the system",
                        new List<string> { "Email already exists in the system" }));
                }

                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = user.Id },
                    ApiResponse<UserDto>.SuccessResponse(user, "User created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                // Business logic validation errors (e.g., duplicate email)
                _logger.LogWarning(ex, "Validation error while creating user");
                return BadRequest(ApiResponse<UserDto>.FailureResponse(
                    ex.Message,
                    new List<string> { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, ApiResponse<UserDto>.FailureResponse(
                    "An error occurred while creating the user",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Updates an existing user's information
        /// Validates input data and ensures email uniqueness
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="updateUserDto">Updated user data</param>
        /// <returns>Updated user if successful, 404 if user not found</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.LogInformation("PUT /api/users/{Id} - Updating user", id);

                // Model validation is handled by data annotations
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState.Values)
                    {
                        foreach (var error in state.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    return BadRequest(ApiResponse<UserDto>.FailureResponse(
                        "Validation failed",
                        errors));
                }

                var user = await _userService.UpdateUserAsync(id, updateUserDto);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.FailureResponse(
                        $"User with ID {id} not found",
                        new List<string> { "User does not exist" }));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                // Business logic validation errors (e.g., duplicate email)
                _logger.LogWarning(ex, "Validation error while updating user with ID {Id}", id);
                return BadRequest(ApiResponse<UserDto>.FailureResponse(
                    ex.Message,
                    new List<string> { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {Id}", id);
                return StatusCode(500, ApiResponse<UserDto>.FailureResponse(
                    "An error occurred while updating the user",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Deletes a user from the system
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>Success status if deleted, 404 if user not found</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("DELETE /api/users/{Id} - Deleting user", id);

                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse<bool>.FailureResponse(
                        $"User with ID {id} not found",
                        new List<string> { "User does not exist" }));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {Id}", id);
                return StatusCode(500, ApiResponse<bool>.FailureResponse(
                    "An error occurred while deleting the user",
                    new List<string> { ex.Message }));
            }
        }
    }
}
