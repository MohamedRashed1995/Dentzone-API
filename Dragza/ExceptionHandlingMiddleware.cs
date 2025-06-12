using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Dragza.API
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
                _logger.LogInformation(context.Request.Path);
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            // Handle specific exception types
            if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "Access denied.";
            }

            _logger.LogError(exception, "An error occurred while processing the request.");
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(exception.InnerException?.ToString() ?? "", EventLogEntryType.Error);
                eventLog.WriteEntry(exception.InnerException?.Message ?? "", EventLogEntryType.Error);
            }
            var response = new
            {
                statusCode = (int)statusCode,
                message,
                error = exception.Message,

            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

}
