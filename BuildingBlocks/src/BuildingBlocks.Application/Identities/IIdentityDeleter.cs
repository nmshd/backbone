using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.Identities;

public interface IIdentityDeleter
{
    Task Delete(IdentityAddress identityAddress);
}
