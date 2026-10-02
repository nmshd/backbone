using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ChangeFeatureFlags;

public class Command : Dictionary<string, bool>, IRequest;
