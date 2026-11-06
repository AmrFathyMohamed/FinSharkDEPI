using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Middleware
{
    // Session 8: Global Exception Handling — بيمسك أي Exception مش متوقّع في التطبيق كله.
    // بيعمل حاجتين: يسجّل التفاصيل الكاملة للمطوّر (Log)، ويرجّع رسالة نضيفة للمستخدم من غير Stack Trace.
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
            CancellationToken cancellationToken)
        {
            // 1. سجّل الـ error بالتفاصيل (للمطوّر) — بيتكتب في الـ Console وفي ملف logs/ بسبب Serilog
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}: {Message}",
                context.Request.Method, context.Request.Path, exception.Message);

            // 2. رجّع ProblemDetails (معيار RFC 7807) — شكل موحّد يفهمه أي client
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = "An unexpected error occurred.",
                Instance = context.Request.Path
            };
            // traceId بيساعدنا نلاقي الـ log المرتبط بالـ request ده
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            // true = الـ exception اتعالج خلاص
            return true;
        }
    }
}
