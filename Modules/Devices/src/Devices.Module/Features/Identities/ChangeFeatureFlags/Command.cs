using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ChangeFeatureFlags;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ChangeFeatureFlagsCommand")]
public class Command : Dictionary<string, bool>, IRequest;
