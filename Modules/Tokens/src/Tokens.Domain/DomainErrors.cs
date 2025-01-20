using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Tokens.Domain;

public static class DomainErrors
{
    public static DomainError TokenNotPersonalized()
    {
        return new DomainError("error.platform.validation.token.tokenNotPersonalized",
            "The token has to be personalized.");
    }

    public static DomainError NoAllocationForIdentity()
    {
        return new DomainError("error.platform.validation.token.noAllocationForIdentity", "The identity doesn't have an allocation for this token.");
    }
}
