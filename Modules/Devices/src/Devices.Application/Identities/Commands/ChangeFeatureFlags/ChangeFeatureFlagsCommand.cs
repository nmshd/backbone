using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;

public class ChangeFeatureFlagsCommand : Dictionary<string, bool>, IRequest;
