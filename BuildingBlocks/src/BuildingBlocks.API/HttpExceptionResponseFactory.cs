using System.Net;
using System.Text.RegularExpressions;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.BuildingBlocks.API;

public sealed class HttpExceptionResponseFactory(IWebHostEnvironment environment)
{
    private const string UNEXPECTED_EXCEPTION_ERROR_CODE = "error.platform.unexpected";
    private const string REQUEST_BODY_TOO_LARGE_ERROR_CODE = "error.platform.requestBodyTooLarge";

    public HttpExceptionResponse Create(Exception exception) => exception switch
    {
        InfrastructureException infrastructureException => new(HttpStatusCode.BadRequest, HttpError.ForProduction(infrastructureException.Code, infrastructureException.Message, "")),
        ApplicationException applicationException => new(GetStatusCode(applicationException), HttpError.ForProduction(applicationException.Code, applicationException.Message, "", GetCustomData(applicationException))),
        DomainException domainException => new(GetStatusCode(domainException), HttpError.ForProduction(domainException.Code, domainException.Message, "")),
        BadHttpRequestException { StatusCode: StatusCodes.Status413PayloadTooLarge } =>
            new(HttpStatusCode.BadRequest, HttpError.ForProduction(REQUEST_BODY_TOO_LARGE_ERROR_CODE, "The request body is too large.", "")),
        BadHttpRequestException badHttpRequestException => CreateInvalidInputResponse(badHttpRequestException),
        _ => new(HttpStatusCode.InternalServerError, CreateUnexpectedExceptionError(exception))
    };

    private static HttpExceptionResponse CreateInvalidInputResponse(BadHttpRequestException exception)
    {
        var error = GenericApplicationErrors.Validation.InputCannotBeParsed(exception.InnerException?.Message ?? exception.Message);
        return new HttpExceptionResponse(HttpStatusCode.BadRequest, HttpError.ForProduction(error.Code, error.Message, ""));
    }

    private HttpError CreateUnexpectedExceptionError(Exception exception)
    {
        if (!environment.IsDevelopment() && !environment.IsLocal())
            return HttpError.ForProduction(UNEXPECTED_EXCEPTION_ERROR_CODE, "An unexpected error occurred.", "");

        var details = exception.Message;
        for (var inner = exception.InnerException; inner != null; inner = inner.InnerException)
            details += "\r\n> " + inner.Message;

        var stackTrace = exception.StackTrace == null ? Enumerable.Empty<string>() : Regex.Matches(exception.StackTrace, "at .+").Select(match => match.Value.Trim());
        return HttpError.ForDev(UNEXPECTED_EXCEPTION_ERROR_CODE, "An unexpected error occurred.", "", stackTrace, details);
    }

    private static dynamic? GetCustomData(ApplicationException exception) => exception is QuotaExhaustedException quotaExhaustedException
        ? quotaExhaustedException.ExhaustedMetricStatuses.Select(status => new
        {
#pragma warning disable IDE0037
            MetricKey = status.MetricKey.Value,
            IsExhaustedUntil = status.IsExhaustedUntil
#pragma warning restore IDE0037
        })
        : exception.AdditionalData;

    private static HttpStatusCode GetStatusCode(ApplicationException exception) => exception switch
    {
        NotFoundException => HttpStatusCode.NotFound,
        ActionForbiddenException => HttpStatusCode.Forbidden,
        QuotaExhaustedException => HttpStatusCode.TooManyRequests,
        _ => HttpStatusCode.BadRequest
    };

    private static HttpStatusCode GetStatusCode(DomainException exception) => exception.Code switch
    {
        var code when code == GenericDomainErrors.NotFound().Code => HttpStatusCode.NotFound,
        var code when code == GenericDomainErrors.Forbidden().Code => HttpStatusCode.Forbidden,
        _ => HttpStatusCode.BadRequest
    };
}

public sealed record HttpExceptionResponse(HttpStatusCode StatusCode, HttpError Error);
