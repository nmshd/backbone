using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsResponse
{
    public required long NewIndex { get; set; }

    public required IEnumerable<PushDatawalletModificationsResponseItem> Modifications { get; set; }
}

public class PushDatawalletModificationsResponseItem : IHaveCustomMapping
{
    public required string Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<DatawalletModification, PushDatawalletModificationsResponseItem>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(x => x.Id.Value));
    }
}
