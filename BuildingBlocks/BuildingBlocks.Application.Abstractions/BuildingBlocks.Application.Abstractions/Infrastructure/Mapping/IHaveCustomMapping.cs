using AutoMapper;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}