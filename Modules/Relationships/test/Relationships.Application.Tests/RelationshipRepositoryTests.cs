using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using Castle.Core.Logging;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests;

public class RelationshipRepositoryTests
{
    private readonly RelationshipsDbContext _arrangeContext;
    private readonly RelationshipsDbContext _actContext;

    public RelationshipRepositoryTests()
    {
        (_arrangeContext, _actContext, _) = FakeDbContextFactory.CreateDbContexts<RelationshipsDbContext>();
    }
    
    [Fact]
    public async Task Test()
    {
        // Arrange
        var identityAddress = Devices.Application.Tests.TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = Devices.Application.Tests.TestDataGenerator.CreateRandomDeviceId();

        var relationshipTemplate = new RelationshipTemplate(identityAddress, deviceId, 0, DateTime.UtcNow.AddDays(10000), new byte[] { 1, 2, 3 });

        var relationship1 = new Relationship(
            relationshipTemplate, identityAddress, deviceId, new byte[] { 1, 2, 3 });

        var relationship2 = new Relationship(
            relationshipTemplate, identityAddress, deviceId, null);

        await _arrangeContext.RelationshipTemplates.AddAsync(relationshipTemplate);
        await _arrangeContext.SaveChangesAsync();

        await _arrangeContext.Relationships.AddAsync(relationship1);
        await _arrangeContext.SaveChangesAsync();

        await _arrangeContext.Relationships.AddAsync(relationship2);
        await _arrangeContext.SaveChangesAsync();

        var fakeBlobStorage = A.Fake<IBlobStorage>();

        A.CallTo(() => fakeBlobStorage.FindAsync(
                A<string>._, $"{relationship2.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id}_Req"))
            .Returns(new byte[] { 9, 8, 7 });

        var relationshipRepository = new RelationshipsRepository(
            _actContext, fakeBlobStorage, A.Fake<IOptions<BlobOptions>>(), A.Fake<ILogger<RelationshipsRepository>>());

        // Act
        var relationshipChange1 = await relationshipRepository.FindRelationshipChange(
            relationship1.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress, CancellationToken.None);
        var relationshipChange2 = await relationshipRepository.FindRelationshipChange(
            relationship2.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress, CancellationToken.None);

        // Assert
        relationshipChange1.Request.Content.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        relationshipChange2.Request.Content.Should().BeEquivalentTo(new byte[] { 9, 8, 7 });
    }

    [Fact]
    public async Task Test2()
    {
        // Arrange
        var identityAddress1 = Devices.Application.Tests.TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = Devices.Application.Tests.TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = Devices.Application.Tests.TestDataGenerator.CreateRandomDeviceId();

        var relationshipTemplate = new RelationshipTemplate(identityAddress1, deviceId, 0, DateTime.UtcNow.AddDays(10000), new byte[] { 1, 2, 3 });

        var relationship1 = new Relationship(
            relationshipTemplate, identityAddress1, deviceId, new byte[] { 1, 2, 3 });

        var relationship2 = new Relationship(
            relationshipTemplate, identityAddress1, deviceId, null);

        relationship1.AcceptChange(relationship1.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress2, deviceId, new byte[] { 3, 2, 1 });
        relationship2.AcceptChange(relationship2.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress2, deviceId, null);

        await _arrangeContext.RelationshipTemplates.AddAsync(relationshipTemplate);
        await _arrangeContext.SaveChangesAsync();

        await _arrangeContext.Relationships.AddAsync(relationship1);
        await _arrangeContext.SaveChangesAsync();

        await _arrangeContext.Relationships.AddAsync(relationship2);
        await _arrangeContext.SaveChangesAsync();

        var fakeBlobStorage = A.Fake<IBlobStorage>();

        A.CallTo(() => fakeBlobStorage.FindAsync(
                A<string>._, $"{relationship2.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id}_Res"))
            .Returns(new byte[] { 9, 8, 7 });

        var relationshipRepository = new RelationshipsRepository(
            _actContext, fakeBlobStorage, A.Fake<IOptions<BlobOptions>>(), A.Fake<ILogger<RelationshipsRepository>>());

        // Act
        var relationshipChange1 = await relationshipRepository.FindRelationshipChange(
            relationship1.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress1, CancellationToken.None);
        var relationshipChange2 = await relationshipRepository.FindRelationshipChange(
            relationship2.Changes.GetLatestOfType(RelationshipChangeType.Creation).Id, identityAddress1, CancellationToken.None);

        // Assert
        relationshipChange1.Response.Content.Should().BeEquivalentTo(new byte[] { 3, 2, 1 });
        relationshipChange2.Response.Content.Should().BeEquivalentTo(new byte[] { 9, 8, 7 });
    }
}
