using Banking.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

namespace Banking.Common.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Type = exception.GetType().Name
        };

        switch (exception)
        {
            case NotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case ValidationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case InsufficientFundsException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = $"Insufficient funds: {exception.Message}";
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An internal server error occurred";
                break;
        }

        if (_env.IsDevelopment())
        {
            response.Details = exception.ToString();
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }

    private class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
