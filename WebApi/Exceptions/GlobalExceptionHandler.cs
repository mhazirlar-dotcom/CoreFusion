using Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    #region Fields

    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    #endregion Fields

    #region Methods

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext , Exception exception , CancellationToken cancellationToken)
    {
        int statusCode;
        string title;
        string detail;

        switch (exception)
        {
            case CoreValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Doğrulama hatası.";
                detail = exception.Message;
                _logger.LogWarning(exception , "Doğrulama hatası oluştu.");
                break;

            case NotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                title = "Kayıt bulunamadı.";
                detail = exception.Message;
                _logger.LogWarning(exception , "Kayıt bulunamadı.");
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Beklenmeyen bir hata oluştu.";
                detail = "İşlem sırasında beklenmeyen bir hata meydana geldi.";
                _logger.LogError(exception , "Beklenmeyen bir hata oluştu.");
                break;
        }

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails , cancellationToken);

        return true;
    }

    #endregion Methods
}