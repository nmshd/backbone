using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.Shouldly.Extensions;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Datawallet.Queries.GetDatawalletModifications;

public class HandlerTests : AbstractTestsBase
{
    private const ushort DATAWALLET_VERSION = 1;

    private static readonly IdentityAddress ACTIVE_IDENTITY = CreateRandomIdentityAddress();

    private readonly SynchronizationDbContext _arrangeContext;
    private readonly SynchronizationDbContext _actContext;
    private readonly Handler _handler;

    public HandlerTests()
    {
        (_arrangeContext, _actContext, _) = FakeDbContextFactory.CreateDbContexts<SynchronizationDbContext>();
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
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(1);
        result.First().Id.ShouldBe(latestCacheChangedModification.Id);
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
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(1);
        result.First().Id.ShouldBe(latestUpdateModification.Id);
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
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(2);
    }

    [Fact]
    public async Task Does_not_combine_modifications_with_different_ids()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(2);
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
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(2);
    }

    [Fact]
    public async Task Does_not_return_modifications_of_another_identity()
    {
        // Arrange
        _arrangeContext.SaveEntity(CreateDatawalletForActiveIdentity());
        var anotherIdentity = CreateRandomIdentityAddress();
        var datawallet = CreateDatawalletFor(anotherIdentity);
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters());
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(0);
    }


    [Fact]
    public async Task Handles_complex_case_correctly()
    {
        // Arrange
        var datawalletOfActiveIdentity = CreateDatawalletForActiveIdentity();
        var anotherIdentity = CreateRandomIdentityAddress();
        var datawalletOfAnotherIdentity = CreateDatawalletFor(anotherIdentity);
        var modificationOfAnotherIdentity = datawalletOfAnotherIdentity.AddModification(new DatawalletExtensions.AddModificationParameters());

        var createId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category1"
        });
        var createId1Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category2"
        });
        var createId2Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "oid2",
            PayloadCategory = "category1"
        });
        var createId2Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Create,
            ObjectIdentifier = "oid2",
            PayloadCategory = "category2"
        });
        var firstUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category1"
        });
        var updateId1Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category2"
        });
        var updateId2Category2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid2",
            PayloadCategory = "category2"
        });
        var secondUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category1"
        });
        var deleteId2 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Delete,
            ObjectIdentifier = "oid2"
        });
        var lastUpdateId1Category1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Update,
            ObjectIdentifier = "oid1",
            PayloadCategory = "category1"
        });
        var deleteId1 = datawalletOfActiveIdentity.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            Type = DatawalletModificationType.Delete,
            ObjectIdentifier = "oid1"
        });

        _arrangeContext.SaveEntity(datawalletOfAnotherIdentity);
        _arrangeContext.SaveEntity(datawalletOfActiveIdentity);

        // Act
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(9);
        result.ShouldNotContain(m => m.Id == modificationOfAnotherIdentity.Id);
        result.ShouldNotContain(m => m.Id == firstUpdateId1Category1.Id);
        result.ShouldNotContain(m => m.Id == secondUpdateId1Category1.Id);

        result.ShouldContain(m => m.Id == createId1Category1.Id);
        result.ShouldContain(m => m.Id == createId1Category2.Id);
        result.ShouldContain(m => m.Id == createId2Category1.Id);
        result.ShouldContain(m => m.Id == createId2Category2.Id);
        result.ShouldContain(m => m.Id == updateId1Category2.Id);
        result.ShouldContain(m => m.Id == updateId2Category2.Id);
        result.ShouldContain(m => m.Id == deleteId2.Id);
        result.ShouldContain(m => m.Id == lastUpdateId1Category1.Id);
        result.ShouldContain(m => m.Id == deleteId1.Id);
    }

    [Fact]
    public async Task Only_returns_modifications_after_given_local_index()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new ListModificationsQuery(0, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(1);
    }

    [Fact]
    public async Task Paginates_returned_results()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid2"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid3"
        });
        _arrangeContext.SaveEntity(datawallet);

        const int pageSize = 2;

        // Act
        var firstPage = await _handler.Handle(new ListModificationsQuery(new PaginationFilter(1, pageSize), null, DATAWALLET_VERSION), CancellationToken.None);
        var secondPage = await _handler.Handle(new ListModificationsQuery(new PaginationFilter(2, pageSize), null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        firstPage.ShouldHaveCount(2);
        firstPage.Pagination.PageSize.ShouldBe(2);
        firstPage.Pagination.PageNumber.ShouldBe(1);
        firstPage.Pagination.TotalPages.ShouldBe(2);
        firstPage.Pagination.TotalRecords.ShouldBe(3);

        secondPage.ShouldHaveCount(1);
        secondPage.Pagination.PageSize.ShouldBe(2);
        secondPage.Pagination.PageNumber.ShouldBe(2);
        secondPage.Pagination.TotalPages.ShouldBe(2);
        secondPage.Pagination.TotalRecords.ShouldBe(3);
    }

    [Fact]
    public async Task Returns_all_modifications_when_passing_no_local_index()
    {
        // Arrange
        var datawallet = CreateDatawalletForActiveIdentity();
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid1"
        });
        datawallet.AddModification(new DatawalletExtensions.AddModificationParameters
        {
            ObjectIdentifier = "oid2"
        });
        _arrangeContext.SaveEntity(datawallet);

        // Act
        var result = await _handler.Handle(new ListModificationsQuery(null, DATAWALLET_VERSION), CancellationToken.None);

        // Assert
        result.ShouldHaveCount(2);
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

        return new Handler(_actContext, userContext);
    }
}
