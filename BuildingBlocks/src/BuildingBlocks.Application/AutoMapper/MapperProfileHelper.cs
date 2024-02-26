using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.BuildingBlocks.Application.AutoMapper;

internal static class MapperProfileHelper
{
    public static IEnumerable<Map> LoadStandardMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();

        var mapsFrom = (
            from type in types
            from @interface in type.GetInterfaces()
            where
                @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                !type.IsAbstract &&
                !type.IsInterface
            select new Map(@interface.GetGenericArguments().First(), type)).ToList();

        return mapsFrom;
    }

    public static IEnumerable<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();

        var mapsFrom = (
            from type in types
            from @interface in type.GetInterfaces()
            where
                typeof(IHaveCustomMapping).IsAssignableFrom(@interface) &&
                !type.IsAbstract &&
                !type.IsInterface
            select (IHaveCustomMapping)Activator.CreateInstance(type)!).ToList();

        return mapsFrom;
    }
}

public sealed class Map
{
    public Map(Type source, Type destination)
    {
        Source = source;
        Destination = destination;
    }

    public Type Source { get; set; }
    public Type Destination { get; set; }
}
