using MediatR;

namespace Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;

public interface IHousekeepingCommand : IRequest<HousekeeperResponse>;

public class HousekeeperResponse
{
    public required int NumberOfDeletedEntities { get; init; }
    public required string EntityNamePlural { get; init; }
}
