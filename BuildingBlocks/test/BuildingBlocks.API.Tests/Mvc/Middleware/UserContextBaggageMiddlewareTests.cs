using Backbone.BuildingBlocks.API.Mvc.Middleware;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Tests.Mvc.Middleware;

public class UserContextBaggageMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SetsUserContextValuesAsBaggage()
    {
        var identityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var userContext = new TestUserContext(identityAddress, deviceId, "client-id", "username");
        var previousBaggage = Baggage.Current;

        try
        {
            Baggage.ClearBaggage();

            var middleware = new UserContextBaggageMiddleware(_ =>
            {
                Baggage.GetBaggage("deviceId").ShouldBe(deviceId.Value);
                Baggage.GetBaggage("identityAddress").ShouldBe(identityAddress.Value);
                Baggage.GetBaggage("clientId").ShouldBe("client-id");
                Baggage.GetBaggage("username").ShouldBe("username");

                return Task.CompletedTask;
            });

            await middleware.InvokeAsync(new DefaultHttpContext(), userContext);
        }
        finally
        {
            Baggage.Current = previousBaggage;
        }
    }

    private class TestUserContext : IUserContext
    {
        private readonly IdentityAddress? _address;
        private readonly DeviceId? _deviceId;
        private readonly string? _clientId;
        private readonly string? _username;

        public TestUserContext(IdentityAddress? address, DeviceId? deviceId, string? clientId, string? username)
        {
            _address = address;
            _deviceId = deviceId;
            _clientId = clientId;
            _username = username;
        }

        public IdentityAddress GetAddress()
        {
            return GetAddressOrNull() ?? throw new NotFoundException();
        }

        public IdentityAddress? GetAddressOrNull()
        {
            return _address;
        }

        public DeviceId GetDeviceId()
        {
            return GetDeviceIdOrNull() ?? throw new NotFoundException();
        }

        public DeviceId? GetDeviceIdOrNull()
        {
            return _deviceId;
        }

        public string GetUserId()
        {
            return GetUserIdOrNull() ?? throw new NotFoundException();
        }

        public string? GetUserIdOrNull()
        {
            return null;
        }

        public string GetUsername()
        {
            return GetUsernameOrNull() ?? throw new NotFoundException();
        }

        public string? GetUsernameOrNull()
        {
            return _username;
        }

        public string GetClientId()
        {
            return GetClientIdOrNull() ?? throw new NotFoundException();
        }

        public string? GetClientIdOrNull()
        {
            return _clientId;
        }
    }
}
