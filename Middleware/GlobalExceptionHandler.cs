using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken c
            )
        {
            _logger.LogError(exception, "Exception {Message}", exception.Message);
            var Problem = new ProblemDetails
            {
                Type = "Server Fail",
                Status = 500,
                Title = "Server Error",
                Detail = "An Exception"
            };
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(Problem, c);
            return true;
        }
    }
}
