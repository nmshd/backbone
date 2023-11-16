﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminUi.Controllers.Filters;

public class RedirectAntiforgeryValidationFailedResultFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult result
            && result.StatusCode == 400
            && result.Value is ProblemDetails problemDetails
            )
        {
            problemDetails.Detail = "xsrf-token-may-be-invalid";
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    { }
}
