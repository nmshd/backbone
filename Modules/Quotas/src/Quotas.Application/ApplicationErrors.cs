using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Quotas.Application;
public class ApplicationErrors
{
    public static ApplicationError QueryAddressDoesNotMatchTheCurrentUser(string queryAddress = "")
    {
        return new ApplicationError("error.platform.validation.quota.queryAddressDoesNotMatchTheCurrentUser", 
            $"The identity address {queryAddress} passed through the URL does not match the current user's identity address.");
    }
}
