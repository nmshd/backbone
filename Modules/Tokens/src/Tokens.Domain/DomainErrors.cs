using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Tokens.Domain;

public class DomainErrors
{
    public static DomainError TokenNotPersonalized()
    {
        return new DomainError("error.platform.validation.token.tokenNotPersonalized",
            "The token has to be personalized.");
    }
}
