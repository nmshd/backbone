namespace Backbone.Tooling.Extensions;

public static class ObjectExtensions
{
    public static void CopyInto<T>(this T? source, T? target) where T : class
    {
        if (source == null || target == null)
            return;

        var sourceProperties = typeof(T).GetProperties();
        var targetProperties = typeof(T).GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
            if (targetProperty != null && targetProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, value);
            }
        }
    }
}
