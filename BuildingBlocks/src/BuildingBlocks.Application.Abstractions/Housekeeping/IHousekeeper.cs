using MediatR;

namespace Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;

public interface IHousekeeper
{
    Task<HousekeepingResponse> Execute(CancellationToken cancellationToken);
}
