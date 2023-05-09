using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConsumerAPI.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static void AddModelError(this ModelStateDictionary modelState, string errorMessage)
    {
        modelState.AddModelError(string.Empty, errorMessage);
    }
}