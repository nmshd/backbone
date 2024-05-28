using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Datawallet.Queries.GetDatawalletModifications;

public class HandlerTests : AbstractTestsBase
{
    private const ushort DATAWALLET_VERSION = 1;

    private static readonly IdentityAddress ACTIVE_IDENTITY = TestDataGenerator.CreateRandomIdentityAddress();

    private readonly SynchronizationDbContext _arrangeContext;
    private readonly SynchronizationDbContext _actContext;
    private readonly Handler _handler;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        (_arrangeContext, _, _actContext) = FakeDbContextFactory.CreateDbContexts<SynchronizationDbContext>();
        _handler = CreateHandler();
    }

    [Fact]
    public async Task Combines_multiple_cacheChanged_modifications()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.CacheChanged
        });
        var latestCacheChangedModification = datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.CacheChanged
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(latestCacheChangedModification.Id);
    }

    [Fact]
    public async Task Combines_multiple_update_modifications()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update
        });
        var latestUpdateModification = datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(latestUpdateModification.Id);
    }

    [Fact]
    public async Task Does_not_combine_modifications_of_different_types()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.CacheChanged
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Does_not_combine_modifications_with_different_ids()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Does_not_combine_modifications_with_different_payload_categories()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            PayloadCategory = "category1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            PayloadCategory = "category2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Does_not_return_modifications_of_another_identity()
    {
        // Arrange
        _arrangeContext.SaveEntity(CreateDatawalletForActiveIdentity());
        var anotherIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var datawallet = CreateDatawalletFor(anotherIdentity);
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters());
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }


    [Fact]
    public async Task Handles_complex_case_correctly()
    {
        // Arrange
        var datawalletOfActiveIdentity = CreateDatawalletForActiveIdentity();
        var anotherIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var datawalletOfAnotherIdentity = CreateDatawalletFor(anotherIdentity);
        var modificationOfAnotherIdentity = datawalletOfAnotherIdentity.AddModification(new DatawalletExtensions.AddModificationParameters());

        var createId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "id1",
            PayloadCategory = "category1"
        });
        var createId1Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "id1",
            PayloadCategory = "category2"
        });
        var createId2Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "id2",
            PayloadCategory = "category1"
        });
        var createId2Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "id2",
            PayloadCategory = "category2"
        });
        var firstUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id1",
            PayloadCategory = "category1"
        });
        var updateId1Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id1",
            PayloadCategory = "category2"
        });
        var updateId2Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id2",
            PayloadCategory = "category2"
        });
        var secondUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id1",
            PayloadCategory = "category1"
        });
        var deleteId2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Delete,
            ObjectIdentifier = "id2"
        });
        var lastUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "id1",
            PayloadCategory = "category1"
        });
        var deleteId1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Delete,
            ObjectIdentifier = "id1"
        });

        _arrangeContext.SaveEntity(datawalletOfAnotherIdentity);
        _arrangeContext.SaveEntity(datawalletOfActiveIdentity);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(9);
        result.Should().NotContain(m => m.Id == modificationOfAnotherIdentity.Id);
        result.Should().NotContain(m => m.Id == firstUpdateId1Category1.Id);
        result.Should().NotContain(m => m.Id == secondUpdateId1Category1.Id);

        result.Should().Contain(m => m.Id == createId1Category1.Id);
        result.Should().Contain(m => m.Id == createId1Category2.Id);
        result.Should().Contain(m => m.Id == createId2Category1.Id);
        result.Should().Contain(m => m.Id == createId2Category2.Id);
        result.Should().Contain(m => m.Id == updateId1Category2.Id);
        result.Should().Contain(m => m.Id == updateId2Category2.Id);
        result.Should().Contain(m => m.Id == deleteId2.Id);
        result.Should().Contain(m => m.Id == lastUpdateId1Category1.Id);
        result.Should().Contain(m => m.Id == deleteId1.Id);
    }

    [Fact]
    public async Task Only_returns_modifications_after_given_local_index()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(0, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Paginates_returned_results()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id2"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id3"
        });
        _arrangeContext.SaveEntity(datawallet);

        const int pageSize = 2;

        // Act
        var firstPage = await _handler.Handle(new GetModificationsQuery(new PaginationFilter(1, pageSize), null, DATAWALLET_VERSION), CancellationToken.None);
        var secondPage = await _handler.Handle(new GetModificationsQuery(new PaginationFilter(2, pageSize), null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        firstPage.Should().HaveCount(2);
        firstPage.Pagination.PageSize.Should().Be(2);
        firstPage.Pagination.PageNumber.Should().Be(1);
        firstPage.Pagination.TotalPages.Should().Be(2);
        firstPage.Pagination.TotalRecords.Should().Be(3);

        secondPage.Should().HaveCount(1);
        secondPage.Pagination.PageSize.Should().Be(2);
        secondPage.Pagination.PageNumber.Should().Be(2);
        secondPage.Pagination.TotalPages.Should().Be(2);
        secondPage.Pagination.TotalRecords.Should().Be(3);
    }

    [Fact]
    public async Task Returns_all_modifications_when_passing_no_local_index()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "id2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new GetModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    private static Domain.Entities.Datawallet CreateDatawalletForActiveIdentity(ushort version = DATAWALLET_VERSION)
    {
        return new Domain.Entities.Datawallet(new Domain.Entities.Datawallet.DatawalletVersion(version), ACTIVE_IDENTITY);
    }

    private static Domain.Entities.Datawallet CreateDatawalletFor(IdentityAddress owner)
    {
        return new Domain.Entities.Datawallet(new Domain.Entities.Datawallet.DatawalletVersion(1), owner);
    }


    private Handler CreateHandler()
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(ACTIVE_IDENTITY);

        return new Handler(_actContext, AutoMapperProfile.CreateMapper(), userContext);
    }
}
