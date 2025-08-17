using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace webapi.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "WebAPI")
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logPath, "application-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    new JsonFormatter(),
                    path: Path.Combine(logPath, "application-.json"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .WriteTo.File(
                    path: Path.Combine(logPath, "errors-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }

        public static void ConfigureRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode > 399
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
                    
                    if (httpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                        diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? 
                                                      httpContext.User.FindFirst("userId")?.Value);
                    }
                };
            });
        }
    }
}