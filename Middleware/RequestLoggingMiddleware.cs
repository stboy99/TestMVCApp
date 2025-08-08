using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestMVCApp.Middleware
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
            var path = context.Request.Path;

            await _next(context); // Call the next middleware

            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Request to {Path} took {Time}ms", path, elapsed);
        }
    }
}
