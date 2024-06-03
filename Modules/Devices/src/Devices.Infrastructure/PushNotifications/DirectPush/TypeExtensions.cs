using System.Reflection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public static class TypeExtensions
{
    public static T? GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T?)type.GetCustomAttribute(typeof(T));
    }
}
