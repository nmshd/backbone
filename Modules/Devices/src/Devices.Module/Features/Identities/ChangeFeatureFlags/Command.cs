using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ChangeFeatureFlags;

public class ChangeFeatureFlagsCommand : Dictionary<string, bool>, IRequest;
