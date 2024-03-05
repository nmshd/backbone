using System.Reflection;
using AutoMapper;

namespace Backbone.BuildingBlocks.Application.AutoMapper;

public abstract class AutoMapperProfileBase : Profile
{
    private readonly Assembly _assemblyWithTypes;

    protected AutoMapperProfileBase(Assembly assemblyWithTypes)
    {
        _assemblyWithTypes = assemblyWithTypes;

        LoadStandardMappings();
        LoadCustomMappings();
        LoadConverters();

        AllowNullCollections = true;
    }

    private void LoadConverters()
    {
    }

    private void LoadStandardMappings()
    {
        var mapsFrom = MapperProfileHelper.LoadStandardMappings(_assemblyWithTypes);

        foreach (var map in mapsFrom) CreateMap(map.Source, map.Destination).ReverseMap();
    }

    private void LoadCustomMappings()
    {
        var mapsFrom = MapperProfileHelper.LoadCustomMappings(_assemblyWithTypes);

        foreach (var map in mapsFrom) map.CreateMappings(this);
    }
}
