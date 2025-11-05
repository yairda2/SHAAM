using System.Collections.Generic;

namespace UserManagementAPI.Common
{
    /// <summary>
    /// Generic wrapper for API responses providing consistent structure across all endpoints
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The data payload returned from the operation
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// User-friendly message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Collection of error messages if the operation failed
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Errors = null
            };
        }

        /// <summary>
        /// Creates a failure response with error messages
        /// </summary>
        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
