using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Common;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Global exception handling middleware for catching and processing unhandled exceptions
    /// Provides consistent error response format across the application
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue processing the request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception with full stack trace
                _logger.LogError(ex, "An unhandled exception occurred while processing the request");

                // Handle the exception and return appropriate response
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions and formats them into consistent API responses
        /// Determines HTTP status code based on exception type
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred";

            // Determine status code and message based on exception type
            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access";
                    break;

                default:
                    // For unknown exceptions, use generic message in production
                    // In development, include exception details
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            // Create error response
            var response = ApiResponse<object>.FailureResponse(
                message,
                new System.Collections.Generic.List<string> { exception.Message });

            // Serialize and write response
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
