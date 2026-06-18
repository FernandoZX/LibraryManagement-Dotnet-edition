using LibraryManagement.Domain.Common;
using System.Net;
using System.Text.Json;

namespace LibraryManagement.Api.Middleware
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
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain rule violated");
                await WriteProblem(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static Task WriteProblem(HttpContext context, HttpStatusCode status, string detail)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { status = (int)status, detail }));
        }
    }
}
