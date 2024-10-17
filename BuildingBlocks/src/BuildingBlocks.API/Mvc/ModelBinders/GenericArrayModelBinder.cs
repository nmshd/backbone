using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Org.BouncyCastle.Utilities.Encoders;

namespace Backbone.BuildingBlocks.API.Mvc.ModelBinders;

public class GenericArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var elementType = bindingContext.ModelType.GetElementType()!;
        var templates = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
        var query = bindingContext.HttpContext.Request.Query;

        for (var i = 0;; i++)
        {
            var instance = Activator.CreateInstance(elementType);
            var properties = elementType.GetProperties();

            var found = false;
            foreach (var property in properties)
            {
                var key = $"templates.{i}.{property.Name.ToLower()}";
                if (query.ContainsKey(key))
                {
                    var value = property.PropertyType.IsAssignableTo(typeof(byte[])) ? Base64.Decode(query[key].ToString()) : Convert.ChangeType(query[key].ToString(), property.PropertyType);

                    property.SetValue(instance, value);
                    found = true;
                }
            }

            if (!found)
                break;

            templates.Add(instance);
        }

        var resultArray = Array.CreateInstance(elementType, templates.Count);

        for (var i = 0; i < templates.Count; i++)
        {
            resultArray.SetValue(templates[i], i);
        }

        bindingContext.Result = ModelBindingResult.Success(resultArray);
        return Task.CompletedTask;
    }
}

public class GenericArrayModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.IsEnumerableType && context.Metadata.ElementMetadata != null && context.Metadata.ElementMetadata.IsComplexType)
        {
            return new BinderTypeModelBinder(typeof(GenericArrayModelBinder));
        }

        return null;
    }
}
