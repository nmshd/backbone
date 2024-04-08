using AutoMapper;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile configuration);
}
