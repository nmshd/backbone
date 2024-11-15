using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

public interface IUserContext
{
    IdentityAddress GetAddress();
    IdentityAddress? GetAddressOrNull();

    DeviceId GetDeviceId();
    DeviceId? GetDeviceIdOrNull();

    string GetUserId();
    string? GetUserIdOrNull();

    string GetUsername();
    string? GetUsernameOrNull();

    string GetClientId();
    string? GetClientIdOrNull();
}
