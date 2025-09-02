namespace Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;

public record HousekeepingResponse
{
    public required List<HousekeepingResponseItem> Items { get; init; }
}

public record HousekeepingResponseItem
{
    public required Type EntityType { get; init; }
    public required int NumberOfDeletedEntities { get; init; }
}
