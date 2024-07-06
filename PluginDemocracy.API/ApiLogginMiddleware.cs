using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PluginDemocracy.API
{
    public class ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ApiLoggingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Log the request
                _logger.LogInformation("Incoming request: {Method} {Url}", context.Request.Method, context.Request.Path);

                // Copy the original response stream
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    // Log the response
                    _logger.LogInformation("Outgoing response: {StatusCode}", context.Response.StatusCode);

                    // Check if the response is a 400 Bad Request and log model state errors
                    if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                    {
                        LogModelStateErrors(context);
                    }

                    // Copy the contents of the new memory stream (which contains the response) to the original stream
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
        private async Task LogModelStateErrors(HttpContext context)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.LogError($"Request Body: {body}");

            if (context.RequestServices.GetService(typeof(ModelStateDictionary)) is ModelStateDictionary modelState)
            {
                if (!modelState.IsValid)
                {
                    foreach (var state in modelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogError("Model state error in {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}
public class ModelStateFeatureFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var modelState = context.ModelState;
        httpContext.Features.Set(modelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No implementation needed
    }
}
