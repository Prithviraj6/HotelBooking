using System.Diagnostics;
using System.Security.Claims;

namespace HotelBooking.API.Middleware
{
    /// <summary>
    /// Logs every HTTP request with:
    /// - Correlation ID (from X-Correlation-Id header or ASP.NET TraceIdentifier)
    /// - Authenticated User ID (from JWT claim if present)
    /// - Method, Path, Status, Elapsed time
    /// 4xx responses → Warning | 5xx responses → Error | 2xx/3xx → Information
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Correlation ID — use incoming header or fall back to ASP.NET's built-in trace ID
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? context.TraceIdentifier;

            context.Response.Headers["X-Correlation-Id"] = correlationId;

            var stopwatch = Stopwatch.StartNew();

            // Log the incoming request
            _logger.LogInformation(
                "⟶  {Method} {Path} | CorrelationId={CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await _next(context);

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anon";
            var elapsed = stopwatch.ElapsedMilliseconds;

            // Choose log level based on status code
            if (statusCode >= 500)
            {
                _logger.LogError(
                    "⟵  {Method} {Path} → {StatusCode} in {Elapsed}ms | UserId={UserId} | CorrelationId={CorrelationId}",
                    context.Request.Method, context.Request.Path,
                    statusCode, elapsed, userId, correlationId);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "⟵  {Method} {Path} → {StatusCode} in {Elapsed}ms | UserId={UserId} | CorrelationId={CorrelationId}",
                    context.Request.Method, context.Request.Path,
                    statusCode, elapsed, userId, correlationId);
            }
            else
            {
                _logger.LogInformation(
                    "⟵  {Method} {Path} → {StatusCode} in {Elapsed}ms | UserId={UserId} | CorrelationId={CorrelationId}",
                    context.Request.Method, context.Request.Path,
                    statusCode, elapsed, userId, correlationId);
            }
        }
    }
}