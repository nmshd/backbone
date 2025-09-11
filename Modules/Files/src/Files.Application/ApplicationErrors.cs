using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Files.Application;

public static class ApplicationErrors
{
    public static ApplicationError CannotClaimOwnershipOfOwnFile()
    {
        return new ApplicationError("error.platform.validation.file.cannotClaimOwnershipOfOwnFile",
            "You cannot claim the ownership of a file you already own.");
    }
}
