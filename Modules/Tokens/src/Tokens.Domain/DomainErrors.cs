using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Tokens.Domain;

public class DomainErrors
{
    public static DomainError TokenNotPersonalized()
    {
        return new DomainError("error.platform.validation.token.tokenNotPersonalized",
            "The token has to be personalized.");
    }

    public static DomainError NoAllocationForOwner()
    {
        return new DomainError("error.platform.validation.token.noAllocationForOwner", "The owner of the token cannot be allocated.");
    }

    public static DomainError AlreadyAllocated()
    {
        return new DomainError("error.platform.validation.token.alreadyAllocated", "The identity already has an allocation for this token.");
    }
}
