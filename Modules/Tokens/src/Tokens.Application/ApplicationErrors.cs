using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Tokens.Application;

public class ApplicationErrors
{
    public static ApplicationError TokenIsLocked()
    {
        return new ApplicationError("error.platform.token.tooManyFailedPasswordAttempts", "Too many wrong password attempts, the token is locked.");
    }
}
