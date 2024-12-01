using System.Security.Authentication;
using System.Security;

namespace CoreAppStructure.Core.Middlewares
{
    // Middleware này sẽ bắt tất cả các ngoại lệ không xử lý trong ứng dụng và trả về phản hồi phù hợp.
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "ALL", $"/api", null);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error",
                Detailed = exception.Message // Có thể loại bỏ trong môi trường production.
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var jsonResponse = JsonSerializer.Serialize(response);
            LogHelper.LogError(_logger, null, "ALL", $"/api",jsonResponse);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
