using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class IdentitiesContext
{
    public readonly Dictionary<string, StartDeletionProcessResponse> StartDeletionProcessResponses = new();
}
