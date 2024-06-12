using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using static Backbone.Modules.Devices.Infrastructure.Tests.TestDataGenerator;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Sse;

public class ServerSentEventsConnectorTests : AbstractTestsBase
{
    [Fact]
    public async Task Can_handle_empty_list_of_registrations()
    {
        // Arrange
        var sseConnector = CreateConnector();

        // Act
        var results = await sseConnector.Send([], CreateRandomIdentityAddress(), new TestPushNotification());

        // Assert
        results.Successes.Should().BeEmpty();
        results.Failures.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_a_success_if_a_registration_is_passed()
    {
        // Arrange
        var sseConnector = CreateConnector();

        // Act
        var results = await sseConnector.Send([CreatePnsRegistrationForSse()], CreateRandomIdentityAddress(), new TestPushNotification());

        // Assert
        results.Successes.Should().HaveCount(1);
        results.Failures.Should().BeEmpty();
    }

    [Fact]
    public async Task Uses_the_SseServerClient_to_send_the_event()
    {
        // Arrange
        var recipient = CreateRandomIdentityAddress();

        var mockSseServerClient = A.Fake<ISseServerClient>();

        var sseConnector = CreateConnector(mockSseServerClient);

        // Act
        await sseConnector.Send([CreatePnsRegistrationForSse()], recipient, new TestPushNotification());

        // Assert
        A.CallTo(() => mockSseServerClient.SendEvent(recipient, A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_the_push_notification_name_to_the_SseServerClient()
    {
        // Arrange
        var recipient = CreateRandomIdentityAddress();

        var mockSseServerClient = A.Fake<ISseServerClient>();

        var sseConnector = CreateConnector(mockSseServerClient);

        // Act
        await sseConnector.Send([CreatePnsRegistrationForSse()], recipient, new TestPushNotification());

        // Assert
        A.CallTo(() => mockSseServerClient.SendEvent(A<string>._, "Test")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Returns_a_failure_with_reason_InvalidHandle_if_SseServerClient_throws_a_SseClientNotRegisteredException()
    {
        // Arrange
        var recipient = CreateRandomIdentityAddress();

        var fakeSseServerClient = new SseServerClientThrowing<SseClientNotRegisteredException>();

        var sseConnector = CreateConnector(fakeSseServerClient);

        // Act
        var results = await sseConnector.Send([CreatePnsRegistrationForSse()], recipient, new TestPushNotification());

        // Assert
        results.Failures.Should().HaveCount(1);
        results.Successes.Should().BeEmpty();
        results.Failures.First().Error!.Reason.Should().Be(ErrorReason.InvalidHandle);
    }

    [Fact]
    public async Task Returns_a_failure_with_reason_Unexpected_if_SseServerClient_throws_an_unexpected_exception()
    {
        // Arrange
        var recipient = CreateRandomIdentityAddress();

        var fakeSseServerClient = new SseServerClientThrowing<Exception>();

        var sseConnector = CreateConnector(fakeSseServerClient);

        // Act
        var results = await sseConnector.Send([CreatePnsRegistrationForSse()], recipient, new TestPushNotification());

        // Assert
        results.Failures.Should().HaveCount(1);
        results.Successes.Should().BeEmpty();
        results.Failures.First().Error!.Reason.Should().Be(ErrorReason.Unexpected);
    }

    private static ServerSentEventsConnector CreateConnector(ISseServerClient? sseServerClient = null)
    {
        sseServerClient ??= A.Dummy<ISseServerClient>();
        return new ServerSentEventsConnector(sseServerClient, A.Dummy<ILogger<ServerSentEventsConnector>>());
    }

    private class TestPushNotification : IPushNotification;

    private class SseServerClientThrowing<TException> : ISseServerClient where TException : Exception, new()
    {
        public Task SendEvent(string recipient, string eventName)
        {
            throw new TException();
        }
    }
}
