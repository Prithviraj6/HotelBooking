using FluentValidation;
using HotelBooking.Application.DTOs.Common;
using System.Net;
using System.Security.Claims;

namespace HotelBooking.API.Middleware
{
    /// <summary>
    /// Global exception handler — maps known exception types to structured ApiResponse envelopes.
    /// Runs first in the pipeline to catch all unhandled exceptions.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // FluentValidation throws this when auto-validation is bypassed manually
                var errors = ex.Errors
                    .Select(e => e.ErrorMessage)
                    .Distinct()
                    .ToList();

                _logger.LogWarning(
                    "Validation failed on {Method} {Path} — {ErrorCount} error(s): {Errors}",
                    context.Request.Method,
                    context.Request.Path,
                    errors.Count,
                    string.Join(" | ", errors));

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail("Validation failed.", 400, errors));
            }
            catch (Exception ex)
            {
                var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

                _logger.LogError(ex,
                    "Unhandled {ExceptionType} on {Method} {Path} | UserId={UserId} | {Message}",
                    ex.GetType().Name,
                    context.Request.Method,
                    context.Request.Path,
                    userId,
                    ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = ex switch
            {
                KeyNotFoundException =>
                    ApiResponse<object>.NotFound(ex.Message),

                UnauthorizedAccessException =>
                    ApiResponse<object>.Unauthorized(ex.Message),

                ArgumentException =>
                    ApiResponse<object>.Fail(ex.Message, 400),

                InvalidOperationException =>
                    ApiResponse<object>.Fail(ex.Message, 409),

                _ => ApiResponse<object>.Fail(
                    "An unexpected error occurred. Please try again later.", 500)
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}