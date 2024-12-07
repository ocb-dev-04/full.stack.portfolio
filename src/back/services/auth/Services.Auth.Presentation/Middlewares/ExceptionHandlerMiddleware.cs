using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using CQRS.MediatR.Helper.Exceptions;
using Shared.Common.Helper.Providers;
using Shared.Common.Helper.ErrorsHandler;

using Tickets.Shared.Domain.Exceptions;
using Tickets.Shared.Domain.Internal.ErrorLogs;

namespace Tickets.Presentation.Middlewares;

public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    
    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(next, nameof(next));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch(DbUpdateConcurrencyException)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsync(string.Empty);
        }
        catch(ValidationException exception)
        {
            await HandleBadRequest(exception, context);
        }
        catch (TooManyRequestsException)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync(string.Empty);
        }
        catch (Exception ex)
        {
            await HandlerInternalServerError(ex, context);
        }
    }

    private async Task HandleBadRequest(ValidationException ex, HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occurred"
        };

        if (ex.Errors.Any())
        {
            problemDetails.Extensions["errors"] = ex.Errors
                .Select(
                s =>
                {
                    string capitalized = string.Format("{0}{1}", char.ToUpper(s.ErrorMessage[0]), s.ErrorMessage.Substring(1));
                    return string.Format("{0}{1}", s.PropertyName.ToLower(), capitalized);
                })
                .ToArray();
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandlerInternalServerError(Exception ex, HttpContext context)
    {
        _logger.LogError($"--> Some error ocurred: {0}", ex.InnerException);

        IServiceScope scope = context.RequestServices.CreateScope();
        HttpRequestProvider? httpRequestProvider = scope.ServiceProvider.GetService<HttpRequestProvider>();
        IErrorLogRepository? errorLogRepository = scope.ServiceProvider.GetService<IErrorLogRepository>();

        ArgumentNullException.ThrowIfNull(httpRequestProvider, nameof(httpRequestProvider));
        ArgumentNullException.ThrowIfNull(errorLogRepository, nameof(errorLogRepository));

        Result<ErrorLog> error = ErrorLog.Create(
            ex.Message,
            ex.StackTrace ?? string.Empty,
            in httpRequestProvider);

        await errorLogRepository.CreateAsync(error.Value, context.RequestAborted);

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(string.Empty);
    }
}