using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Org.BouncyCastle.Utilities.Encoders;

namespace Backbone.BuildingBlocks.API.Mvc.ModelBinders;

/**
 * <summary>
 * This model binder allows for binding arrays of complex types from query parameters. The query parameters should be formatted as follows:
 * <code>
 *   paramName.0.propertyName1=value&amp;paramName.0.propertyName2=value&amp;paramName.1.propertyName1=value&amp;paramName.1.propertyName2=value
 * </code>
 * </summary>
 */
public class GenericArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var elementType = bindingContext.ModelType.GetElementType()!;
        var items = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
        var query = bindingContext.HttpContext.Request.Query;

        for (var i = 0; ; i++)
        {
            var instance = Activator.CreateInstance(elementType);
            var properties = elementType.GetProperties();

            var queryValueFound = false;
            foreach (var property in properties)
            {
                var key = $"{bindingContext.ModelName}.{i}.{property.Name.ToLower()}";

                if (!query.TryGetValue(key, out var queryValue))
                    continue;

                object? value;

                if (property.PropertyType.IsAssignableTo(typeof(byte[])))
                    value = Base64.Decode(queryValue.ToString());
                else
                    value = Convert.ChangeType(queryValue.ToString(), property.PropertyType);

                property.SetValue(instance, value);

                queryValueFound = true;
            }

            if (!queryValueFound)
                break;

            items.Add(instance);
        }

        var resultArray = Array.CreateInstance(elementType, items.Count);

        for (var i = 0; i < items.Count; i++)
        {
            resultArray.SetValue(items[i], i);
        }

        bindingContext.Result = ModelBindingResult.Success(resultArray);
        return Task.CompletedTask;
    }
}

public class GenericArrayModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata is
        {
            IsEnumerableType: true,
            ElementMetadata.IsComplexType: true
        } && !context.Metadata.ModelType.IsAssignableTo(typeof(IDictionary))
            ? new BinderTypeModelBinder(typeof(GenericArrayModelBinder))
            : null;
    }
}
