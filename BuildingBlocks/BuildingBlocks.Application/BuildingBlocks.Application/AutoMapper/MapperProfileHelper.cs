using System.Reflection;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Enmeshed.BuildingBlocks.Application.AutoMapper
{
    internal static class MapperProfileHelper
    {
        public static IEnumerable<Map> LoadStandardMappings(Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = (
                from type in types
                from interf in type.GetInterfaces()
                where
                    interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                    !type.IsAbstract &&
                    !type.IsInterface
                select new Map(interf.GetGenericArguments().First(), type)).ToList();

            return mapsFrom;
        }

        public static IEnumerable<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = (
                from type in types
                from interf in type.GetInterfaces()
                where
                    typeof(IHaveCustomMapping).IsAssignableFrom(interf) &&
                    !type.IsAbstract &&
                    !type.IsInterface
                select (IHaveCustomMapping) Activator.CreateInstance(type)!).ToList();

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
}