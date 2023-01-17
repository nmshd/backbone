using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Enmeshed.BuildingBlocks.API.Mvc.ExceptionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private const string ERROR_CODE_UNEXPECTED_EXCEPTION = "error.platform.unexpected";
        private const string ERROR_CODE_REQUEST_BODY_TOO_LARGE = "error.platform.requestBodyTooLarge";

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public override void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";

            HttpError httpError;

            switch (context.Exception)
            {
                case ApplicationException applicationException:
                    _logger.LogInformation(
                        $"An {nameof(ApplicationException)} occurred. Error Code: {applicationException.Code}. Error message: {applicationException.Message}");

                    httpError = CreateHttpErrorApplicationException(applicationException);

                    context.HttpContext.Response.StatusCode =
                        (int) GetStatusCodeForApplicationException(applicationException);

                    break;
                case BadHttpRequestException _:
                    _logger.LogInformation(
                        $"{ERROR_CODE_REQUEST_BODY_TOO_LARGE}: The body of the request is too large.");

                    httpError = HttpError.ForProduction(
                        ERROR_CODE_REQUEST_BODY_TOO_LARGE,
                        "The request body is too large.",
                        "" // TODO: add documentation link
                    );

                    context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                    break;
                default:
                    _logger.LogError(context.Exception,
                        $"Unexpected Error while processing request to {context.HttpContext.Request.GetUri()}");

                    httpError = CreateHttpErrorForUnexpectedException(context);

                    context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                    break;
            }

            context.Result = new JsonResult(HttpResponseEnvelope.CreateError(httpError),
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                });
        }

        private HttpError CreateHttpErrorApplicationException(ApplicationException applicationException)
        {
            var httpError = HttpError.ForProduction(
                applicationException.Code,
                applicationException.Message,
                "" // TODO: add documentation link
            );

            return httpError;
        }

        private HttpStatusCode GetStatusCodeForApplicationException(ApplicationException exception)
        {
            return exception switch
            {
                NotFoundException _ => HttpStatusCode.NotFound,
                ActionForbiddenException _ => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.BadRequest
            };
        }

        private HttpError CreateHttpErrorForUnexpectedException(ExceptionContext context)
        {
            HttpError httpError;

            if (_env.IsDevelopment())
            {
                var details = context.Exception.Message;
                var innerException = context.Exception.InnerException;

                while (innerException != null)
                {
                    details += "\r\n> " + innerException.Message;
                    innerException = innerException.InnerException;
                }

                httpError = HttpError.ForDev(
                    ERROR_CODE_UNEXPECTED_EXCEPTION,
                    "An unexpected error occurred.",
                    "", // TODO: add documentation link
                    GetFormattedStackTrace(context.Exception),
                    details
                );
            }
            else
            {
                httpError = HttpError.ForProduction(
                    ERROR_CODE_UNEXPECTED_EXCEPTION,
                    "An unexpected error occurred.",
                    "" // TODO: add documentation link
                );
            }

            return httpError;
        }

        private IEnumerable<string> GetFormattedStackTrace(Exception exception)
        {
            if (exception.StackTrace == null)
                return Enumerable.Empty<string>();

            return
                Regex.Matches(exception.StackTrace, "at .+").Select(m => m.Value.Trim());
        }
    }
}