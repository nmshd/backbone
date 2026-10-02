using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateQueuedForDeletionTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateQueuedForDeletionTierCommand")]
public class Command : IRequest;
