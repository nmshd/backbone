using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.SeedQueuedForDeletionTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SeedQueuedForDeletionTierCommand")]
public class Command : IRequest;
