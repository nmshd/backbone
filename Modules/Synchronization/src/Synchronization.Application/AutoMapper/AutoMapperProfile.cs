using System.Reflection;
using AutoMapper;
using Backbone.BuildingBlocks.Application.AutoMapper;

namespace Backbone.Modules.Synchronization.Application.AutoMapper;

public class AutoMapperProfile : AutoMapperProfileBase
{
    public AutoMapperProfile() : base(Assembly.GetExecutingAssembly()) { }

    public static IMapper CreateMapper()
    {
        var profile = new AutoMapperProfile();
        var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
        var mapper = config.CreateMapper();
        return mapper;
    }
}
