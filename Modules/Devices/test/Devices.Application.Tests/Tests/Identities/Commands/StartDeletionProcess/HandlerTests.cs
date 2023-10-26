using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using MediatR;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcess;

public class HandlerTests
{
    [Fact]
    public async Task Test()
    {
        // Arrange
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(
                () => fakeIdentitiesRepository.FindByAddress(
                    A<IdentityAddress>._,
                    A<CancellationToken>._,
                    A<bool>._
                )
            )
            .Returns(new Identity("", IdentityAddress.Create(new byte[] { }, "id1"), new byte[] { }, TierId.Generate(), 1));

        var handler = new Handler(fakeIdentitiesRepository);

        // Act
        await handler.Handle(new StartDeletionProcessCommand(IdentityAddress.Create(new byte[] { }, "id1")), CancellationToken.None);

        // Assert
        A.CallTo(
                () => fakeIdentitiesRepository.Update(
                    A<Identity>.That.Matches(i => i.DeletionProcesses.Count == 1),
                    A<CancellationToken>._
                )
            )
            .MustHaveHappenedOnceExactly();
    }
}

public class StartDeletionProcessCommand : IRequest
{
    public StartDeletionProcessCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}

public class Handler : IRequestHandler<StartDeletionProcessCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(StartDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, true);

        identity.StartDeletionProcess();

        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
