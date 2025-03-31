using Microsoft.EntityFrameworkCore;
using System.Net;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.Presentation.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions and returning appropriate responses.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Resource already exists.");
                await HandleUnauthorizedAsync(context, ex);
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "Resource already exists.");
                await HandleAlreadyExistsAsync(context, ex);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found.");
                await HandleNotFoundAsync(context, ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Conflict: The resource was updated by another user.");
                await HandleConflictAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleUnauthorizedAsync(HttpContext context, UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        
        private static Task HandleAlreadyExistsAsync(HttpContext context, AlreadyExistsException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }

        private static Task HandleNotFoundAsync(HttpContext context, KeyNotFoundException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var result = new { message = "Resource not found.", detail = ex.Message };
            return context.Response.WriteAsJsonAsync(result);
        }
        
        private static Task HandleConflictAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return context.Response.WriteAsync("Conflict: The resource was updated by another user.");
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = new
            {
                message = "An internal server error occurred.",
                detail = ex.Message
            };
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
