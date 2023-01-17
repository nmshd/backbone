using Microsoft.AspNetCore.Mvc.ModelBinding;

// ReSharper disable once CheckNamespace
namespace AspNetCoreTools.ExtensionMethods
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, string errorMessage)
        {
            modelState.AddModelError(string.Empty, errorMessage);
        }
    }
}