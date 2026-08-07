using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Services.Domain.Exceptions;

namespace Services.API.Middleware;

public class ExceptionHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandling> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandling(
        RequestDelegate next, 
        ILogger<ExceptionHandling> logger, 
        IHostEnvironment env)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        switch (exception)
        {
            case ValidationException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation error";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = "One or more validation errors occurred";
                problemDetails.Extensions["errors"] = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
        
                _logger.LogWarning("Validation failed: {@Errors}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }), 
                    context.Request.Path, context.TraceIdentifier);
                break;
            
            case NotFoundException ex:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource not found";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = ex.Message;
                
                _logger.LogWarning(ex, 
                    "Entity not found: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;

            case ArgumentException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid argument";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = ex.Message;
                
                _logger.LogWarning(ex, 
                    "Invalid argument: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;

            case InvalidOperationException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid operation";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = ex.Message;
                
                _logger.LogWarning(ex, 
                    "Invalid operation: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;

            case UnauthorizedAccessException ex:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Detail = ex.Message;
                
                _logger.LogWarning("Unauthorized access: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;
            
            case ExistsByNameAndSpecializationException ex:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Duplicate service";
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Detail = ex.Message;
    
                _logger.LogWarning(ex, 
                    "Duplicate service: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;

            case SpecializationAlreadyExistsException ex:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Duplicate specialization";
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Detail = ex.Message;
    
                _logger.LogWarning(ex, 
                    "Duplicate specialization: {Message}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Message, context.Request.Path, context.TraceIdentifier);
                break;

            case DuplicateServiceNamesException ex:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Duplicate service names";
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Detail = ex.Message;
                problemDetails.Extensions["duplicateNames"] = ex.Names;
    
                _logger.LogWarning(ex, 
                    "Duplicate service names: {Names}. Path: {Path}, TraceId: {TraceId}", 
                    ex.Names, context.Request.Path, context.TraceIdentifier);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal server error";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Detail = _env.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later.";
                
                if (_env.IsDevelopment() && exception.InnerException is not null)
                {
                    problemDetails.Extensions["innerException"] = exception.InnerException.Message;
                }
                
                _logger.LogError(exception, 
                    "Unhandled exception: {Message}. Path: {Path}, Method: {Method}, TraceId: {TraceId}", 
                    exception.Message, context.Request.Path, context.Request.Method, context.TraceIdentifier);
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await response.WriteAsync(json);
    }
}