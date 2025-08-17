using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace webapi.Middleware
{
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
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                var userId = GetUserId(context);
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                var requestPath = context.Request.Path;
                var requestMethod = context.Request.Method;

                Log.Error(ex,
                    "Unhandled exception occurred. TraceId: {TraceId}, UserId: {UserId}, Path: {RequestPath}, Method: {RequestMethod}, UserAgent: {UserAgent}, RemoteIP: {RemoteIP}",
                    traceId, userId, requestPath, requestMethod, userAgent, context.Connection.RemoteIpAddress);

                _logger.LogError(ex,
                    "Unhandled exception occurred. TraceId: {TraceId}, UserId: {UserId}, Path: {RequestPath}, Method: {RequestMethod}",
                    traceId, userId, requestPath, requestMethod);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static string? GetUserId(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                return context.User.FindFirst("sub")?.Value ??
                       context.User.FindFirst("userId")?.Value ??
                       context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }
            return null;
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            var problemDetails = exception switch
            {
                ArgumentNullException nullEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = $"Required parameter '{nullEx.ParamName}' was null or missing",
                    Extensions = { ["traceId"] = traceId }
                },
                ArgumentException argEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = argEx.Message,
                    Extensions = { ["traceId"] = traceId }
                },
                UnauthorizedAccessException => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "Access to this resource is forbidden",
                    Extensions = { ["traceId"] = traceId }
                },
                KeyNotFoundException => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = "The requested resource was not found",
                    Extensions = { ["traceId"] = traceId }
                },
                InvalidOperationException invalidEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = invalidEx.Message,
                    Extensions = { ["traceId"] = traceId }
                },
                TimeoutException => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.5",
                    Title = "Request Timeout",
                    Status = (int)HttpStatusCode.RequestTimeout,
                    Detail = "The request timed out",
                    Extensions = { ["traceId"] = traceId }
                },
                _ => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = "An error occurred while processing your request",
                    Extensions = { ["traceId"] = traceId }
                }
            };

            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(json);
        }
    }
}