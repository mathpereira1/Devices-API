using Devices.Application.Exceptions;
using Devices.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;
	private readonly IProblemDetailsService _problemDetailsService;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
	{
		_logger = logger;
		_problemDetailsService = problemDetailsService;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var (status, title) = exception switch
		{
			DomainValidationException => (StatusCodes.Status400BadRequest, "Validation error"),
			DeviceNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
			DeviceInUseException => (StatusCodes.Status409Conflict, "Conflict"),
			_ => (StatusCodes.Status500InternalServerError, "Internal server error")
		};

		string detail;
		if (status == StatusCodes.Status500InternalServerError)
		{
			_logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
			detail = "An unexpected error occurred.";
		}
		else
		{
			detail = exception.Message;
		}

		httpContext.Response.StatusCode = status;

		return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Status = status,
				Title = title,
				Detail = detail
			}
		});
	}
}
