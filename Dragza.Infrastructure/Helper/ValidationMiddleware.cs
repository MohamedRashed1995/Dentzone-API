using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Dragza.Infrastructure.Helper
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next , ILogger<ValidationMiddleware> logger)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                await HandleValidationErrorsAsync(context);
            }
        }

        private async Task HandleValidationErrorsAsync(HttpContext context)
        {
            if (context.Response.StatusCode != StatusCodes.Status400BadRequest)
            {
                _logger.LogError(context.Response.ToString());
                return;

            }

            var modelState = context.Items["ModelState"] as ModelStateDictionary;
            if (modelState == null || modelState.IsValid)
                return;

            var errors = new Dictionary<string, string[]>();
            foreach (var entry in modelState)
            {
                var key = entry.Key;
                var messages = entry.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                errors.Add(key, messages);
            }

            var response = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Errors = errors,
            };
            _logger.LogError(response.ToString());
            _logger.LogError(errors.ToString());

            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}