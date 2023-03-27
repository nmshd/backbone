using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Backbone.Modules.Devices.Application.Tests;
using Enmeshed.UnitTestTools.TestDoubles.Fakes;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Handler = Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities.Handler;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.AutoMapper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class HandlerTests
{
    private static readonly IdentityAddress ActiveIdentity = TestDataGenerator.CreateRandomIdentityAddress();

    private readonly DevicesDbContext _arrangeContext;
    private readonly DevicesDbContext _actContext;
    private readonly Handler _handler;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        (_arrangeContext, _, _actContext) = FakeDbContextFactory.CreateDbContexts<DevicesDbContext>();
        _handler = CreateHandler();
    }

    [Fact]
    public async void Returns_empty_list_when_no_loaded_entities()
    {
        // Arrange
        List<Identity> identitiesList = new();
        _arrangeContext.SaveEntities(identitiesList.ToArray());
        var request = new PaginationFilter();

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Identities.Should().HaveCount(0);
    }

    [Fact]
    public async void Returns_valid_list_when_entities_loaded()
    {
        // Arrange
        var request = new PaginationFilter();
        List<Identity> identitiesList = new()
        {
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            1),
            new(TestDataGenerator.CreateRandomDeviceId(),
            TestDataGenerator.CreateRandomIdentityAddress(),
            TestDataGenerator.CreateRandomBytes(),
            1)
        };

        _arrangeContext.SaveEntities(identitiesList.ToArray());

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Identities.Should().HaveCount(2);
    }

    [Fact]
    public async void Returns_valid_list_when_entities_loaded_and_attributes_match()
    {
        // Arrange
        var request = new PaginationFilter();
        var expectedClientId = TestDataGenerator.CreateRandomDeviceId();
        var expectedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        List<Identity> identitiesList = new()
        {
            new(expectedClientId, expectedAddress, Array.Empty<byte>(), 1)
        };

        _arrangeContext.SaveEntities(identitiesList.ToArray());

        // Act
        var result = await _handler.Handle(new ListIdentitiesQuery(request), CancellationToken.None);

        // Assert
        result.Identities.Should().HaveCount(1);
        result.Identities[0].ClientId.Should().Be(expectedClientId);
        result.Identities[0].Address.Should().Be(expectedAddress);
        result.Identities[0].PublicKey.Should().BeEquivalentTo(Array.Empty<byte>());
        result.Identities[0].IdentityVersion.Should().Be(1);
    }

    private Handler CreateHandler()
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(ActiveIdentity);

        var mapper = AutoMapperProfile.CreateMapper();

        return new Handler(_actContext, userContext, mapper);
    }
}
