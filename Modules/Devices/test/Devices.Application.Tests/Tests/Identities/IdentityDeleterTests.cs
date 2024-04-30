using Backbone.Modules.Devices.Application.Identities;
using Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities;
public class IdentityDeleterTests
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var identityAddress = CreateRandomIdentityAddress();
        var deleter = new IdentityDeleter(mockMediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeletePnsRegistrationsOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }
}
