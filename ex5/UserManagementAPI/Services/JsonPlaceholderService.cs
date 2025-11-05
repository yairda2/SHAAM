using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Common;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Service for integrating with JSONPlaceholder external API to fetch initial user data
    /// Implements retry logic, timeout handling, and graceful error recovery
    /// </summary>
    public class JsonPlaceholderService : IJsonPlaceholderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JsonPlaceholderService> _logger;

        public JsonPlaceholderService(HttpClient httpClient, ILogger<JsonPlaceholderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure HTTP client timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(Constants.HttpRequestTimeoutSeconds);
        }

        /// <summary>
        /// Fetches initial users from JSONPlaceholder API for database seeding
        /// Implements retry logic for transient failures and comprehensive error handling
        /// </summary>
        public async Task<List<User>> FetchInitialUsersAsync()
        {
            var users = new List<User>();
            var retryCount = 0;

            while (retryCount < Constants.MaxRetryAttempts)
            {
                try
                {
                    _logger.LogInformation("Fetching users from JSONPlaceholder API (Attempt {Attempt}/{MaxAttempts})",
                        retryCount + 1, Constants.MaxRetryAttempts);

                    // Make HTTP GET request to JSONPlaceholder API
                    var response = await _httpClient.GetAsync(Constants.JsonPlaceholderApiUrl);
                    response.EnsureSuccessStatusCode();

                    // Read and parse JSON response
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var jsonPlaceholderUsers = JsonSerializer.Deserialize<List<JsonPlaceholderUser>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (jsonPlaceholderUsers == null || jsonPlaceholderUsers.Count == 0)
                    {
                        _logger.LogWarning("No users returned from JSONPlaceholder API");
                        return users;
                    }

                    // Map JSONPlaceholder users to domain User entities
                    foreach (var jsonUser in jsonPlaceholderUsers)
                    {
                        users.Add(MapToUser(jsonUser));
                    }

                    _logger.LogInformation("Successfully fetched {Count} users from JSONPlaceholder API", users.Count);
                    return users;
                }
                catch (HttpRequestException ex)
                {
                    // Network-related errors such as DNS failures, connection issues
                    _logger.LogError(ex, "HTTP request error while fetching users from JSONPlaceholder (Attempt {Attempt})",
                        retryCount + 1);
                    retryCount++;
                }
                catch (TaskCanceledException ex)
                {
                    // Timeout exception
                    _logger.LogError(ex, "Timeout error while fetching users from JSONPlaceholder (Attempt {Attempt})",
                        retryCount + 1);
                    retryCount++;
                }
                catch (JsonException ex)
                {
                    // JSON parsing errors
                    _logger.LogError(ex, "JSON parsing error while processing JSONPlaceholder response");
                    break; // Don't retry for JSON errors
                }
                catch (Exception ex)
                {
                    // Catch-all for unexpected errors
                    _logger.LogError(ex, "Unexpected error while fetching users from JSONPlaceholder");
                    break; // Don't retry for unexpected errors
                }

                // Wait before retrying (exponential backoff)
                if (retryCount < Constants.MaxRetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                    _logger.LogInformation("Waiting {Delay} seconds before retry", delay.TotalSeconds);
                    await Task.Delay(delay);
                }
            }

            // If all retries failed, return empty list to allow app to start
            _logger.LogWarning("Failed to fetch users from JSONPlaceholder after {MaxAttempts} attempts. Application will start with empty user database.",
                Constants.MaxRetryAttempts);
            return users;
        }

        /// <summary>
        /// Maps JSONPlaceholder user model to domain User entity
        /// Extracts company name from nested company object
        /// </summary>
        private User MapToUser(JsonPlaceholderUser jsonUser)
        {
            return new User
            {
                Id = jsonUser.Id,
                Name = jsonUser.Name ?? string.Empty,
                Email = jsonUser.Email ?? string.Empty,
                Phone = jsonUser.Phone,
                Website = jsonUser.Website,
                Company = jsonUser.Company?.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Internal model representing JSONPlaceholder user structure
        /// Mirrors the JSON structure from the external API
        /// </summary>
        private class JsonPlaceholderUser
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Website { get; set; }
            public JsonPlaceholderCompany? Company { get; set; }
        }

        /// <summary>
        /// Internal model representing company data from JSONPlaceholder
        /// </summary>
        private class JsonPlaceholderCompany
        {
            public string? Name { get; set; }
        }
    }
}
