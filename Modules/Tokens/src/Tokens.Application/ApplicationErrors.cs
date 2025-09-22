using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Tokens.Application;

public class ApplicationErrors
{
    public static ApplicationError TokenIsLocked()
    {
        return new ApplicationError("error.platform.token.tooManyFailedPasswordAttempts", "Too many wrong password attempts, the token is locked.");
    }

    public static ApplicationError ContentUpdateNotPossibleBecauseContentIsNotNull()
    {
        return new ApplicationError("error.platform.token.contentUpdateNotPossibleBecauseContentAlreadyExists",
            "The token already has content. You can only update the content of a token if it has no content yet.");
    }
}
