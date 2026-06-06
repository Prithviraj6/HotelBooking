using HotelBooking.Application.DTOs.Common;
using System.Net;

namespace HotelBooking.API.Middleware
{
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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception on {Method} {Path} — {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context, Exception ex)
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