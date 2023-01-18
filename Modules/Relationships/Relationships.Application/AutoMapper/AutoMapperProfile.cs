using System.Reflection;
using AutoMapper;
using Enmeshed.BuildingBlocks.Application.AutoMapper;

namespace Relationships.Application.AutoMapper;

public class AutoMapperProfile : AutoMapperProfileBase
{
    public AutoMapperProfile() : base(Assembly.GetExecutingAssembly()) { }

    public static IMapper CreateMapper()
    {
        var profile = new AutoMapperProfile();
        var config = new MapperConfiguration(cfg => { cfg.AddProfile(profile); });
        var mapper = config.CreateMapper();
        return mapper;
    }
}
