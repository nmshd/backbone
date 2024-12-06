using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler
{
    public class CreateDatawalletModificationsTests
    {
        [Fact]
        public async Task Handle_ShouldProcessIdentitiesWithDatawalletModifications()
        {
            // Arrange
            var datawalletModificationFactory = A.Fake<IDatawalletModificationFactory>();
            var handler = new CreateDatawalletModifications.CommandHandler(datawalletModificationFactory);

            var identities = new List<DomainIdentity>
            {
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0),
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 5, 0),
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 3, 0)
            };

            var expectedTotalDatawalletModifications = identities.Sum(i => i.NumberOfDatawalletModifications);

            var command = new CreateDatawalletModifications.Command(
                identities,
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            A.CallTo(() => datawalletModificationFactory.Create(A<CreateDatawalletModifications.Command>._, A<DomainIdentity>._))
                .MustHaveHappened(3, Times.Exactly);

            datawalletModificationFactory.TotalDatawalletModifications.Should().Be(expectedTotalDatawalletModifications);
        }

        [Fact]
        public async Task Handle_ShouldNotProcessIdentitiesWithoutDatawalletModifications()
        {
            // Arrange
            var datawalletModificationFactory = A.Fake<IDatawalletModificationFactory>();
            var handler = new CreateDatawalletModifications.CommandHandler(datawalletModificationFactory);

            var identities = new List<DomainIdentity>
            {
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 0, 0),
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 0, 0),
                new(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 0, 0)
            };
            var expectedTotalDatawalletModifications = identities.Sum(i => i.NumberOfDatawalletModifications);

            var command = new CreateDatawalletModifications.Command(
                identities,
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            A.CallTo(() => datawalletModificationFactory.Create(A<CreateDatawalletModifications.Command>._, A<DomainIdentity>._))
                .MustNotHaveHappened();

            datawalletModificationFactory.TotalDatawalletModifications.Should().Be(expectedTotalDatawalletModifications);
        }
    }
}
