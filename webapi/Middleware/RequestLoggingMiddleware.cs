using System.Diagnostics;
using System.Security.Claims; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; 
using Serilog;

namespace webapi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? string.Empty;
            var queryString = context.Request.QueryString.Value ?? string.Empty;
            var fullPath = string.IsNullOrEmpty(queryString) ? path : $"{path}{queryString}";
            
            var user = GetUserInfo(context);
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var remoteIP = context.Connection.RemoteIpAddress?.ToString();

            try
            {
                Log.Information("Request started: {Method} {Path} | User: {User} | TraceId: {TraceId} | RemoteIP: {RemoteIP}",
                    method, fullPath, user, traceId, remoteIP);

                _logger.LogInformation("Request started: {Method} {Path} | User: {User} | TraceId: {TraceId}",
                    method, fullPath, user, traceId);

                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                var logLevel = statusCode >= 500 ? LogLevel.Error :
                              statusCode >= 400 ? LogLevel.Warning :
                              LogLevel.Information;

                var serilogLevel = statusCode >= 500 ? Serilog.Events.LogEventLevel.Error :
                                  statusCode >= 400 ? Serilog.Events.LogEventLevel.Warning :
                                  Serilog.Events.LogEventLevel.Information;

                Log.Write(serilogLevel,
                    "Request completed: {Method} {Path} | Status: {StatusCode} | User: {User} | Duration: {ElapsedMs}ms | TraceId: {TraceId} | RemoteIP: {RemoteIP}",
                    method, fullPath, statusCode, user, elapsedMs, traceId, remoteIP);

                _logger.Log(logLevel,
                    "Request completed: {Method} {Path} | Status: {StatusCode} | User: {User} | Duration: {ElapsedMs}ms | TraceId: {TraceId}",
                    method, fullPath, statusCode, user, elapsedMs, traceId);

                if (elapsedMs > 1000)
                {
                    Log.Warning("Slow request detected: {Method} {Path} | Duration: {ElapsedMs}ms | User: {User} | TraceId: {TraceId} | RemoteIP: {RemoteIP}",
                        method, fullPath, elapsedMs, user, traceId, remoteIP);

                    _logger.LogWarning("Slow request detected: {Method} {Path} | Duration: {ElapsedMs}ms | User: {User} | TraceId: {TraceId}",
                        method, fullPath, elapsedMs, user, traceId);
                }
            }
        }

        private static string GetUserInfo(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                            context.User.FindFirst("sub")?.Value ??
                            context.User.FindFirst("userId")?.Value;

                var Email = context.User.FindFirst(ClaimTypes.Name)?.Value ??
                              context.User.FindFirst("name")?.Value ??
                              context.User.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(Email))
                {
                    return $"{Email} (ID: {userId})";
                }
                else if (!string.IsNullOrEmpty(userId))
                {
                    return $"ID: {userId}";
                }
                else if (!string.IsNullOrEmpty(Email))
                {
                    return Email;
                }

                return "Authenticated (Unknown)";
            }

            return "Anonymous";
        }
    }
}