using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backbone.AdminApi.Filters;

public class RedirectAntiforgeryValidationFailedResultFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult { StatusCode: 400, Value: ProblemDetails problemDetails })
        {
            problemDetails.Detail = "xsrf-token-may-be-invalid";
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    { }
}
