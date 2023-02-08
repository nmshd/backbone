using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Enmeshed.BuildingBlocks.API.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static void AddModelError(this ModelStateDictionary modelState, string errorMessage)
    {
        modelState.AddModelError(string.Empty, errorMessage);
    }
}