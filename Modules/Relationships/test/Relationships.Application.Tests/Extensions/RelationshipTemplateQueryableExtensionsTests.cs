using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using Relationships.Application.Extensions;
using Relationships.Domain.Entities;
using Relationships.Infrastructure.Persistence.Database;
using Xunit;

namespace Relationships.Application.Tests.Extensions;

public class RelationshipTemplateQueryableExtensionsTests
{
    private static readonly DateTime Yesterday = DateTime.UtcNow.AddDays(-1);

    private readonly ApplicationDbContext _arrangeContext;
    private readonly ApplicationDbContext _actContext;

    public RelationshipTemplateQueryableExtensionsTests()
    {
        (_arrangeContext, _, _actContext) = FakeDbContextFactory.CreateDbContexts<ApplicationDbContext>();
    }

    [Fact]
    public void NotExpiredFor_DoesNotFilterOutTemplatesForParticipants()
    {
        // Arrange
        var templateCreator = IdentityAddress.Create(new byte[2], "id0");
        var requestCreator = IdentityAddress.Create(new byte[5], "id0");

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, Yesterday, new byte[2]);
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), new byte[2]);


        _arrangeContext.RelationshipTemplates.Add(template);
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();


        // Act
        var result = _actContext
            .RelationshipTemplates
            .AsQueryable()
            .NotExpiredFor(templateCreator)
            .ToList();


        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void NotExpiredFor_DoesNotFilterOutTemplatesWithoutExpiryDate()
    {
        // Arrange
        var templateCreator = TestDataGenerator.CreateRandomAddress();
        var requestCreator = TestDataGenerator.CreateRandomAddress();

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, null, TestDataGenerator.CreateRandomBytes());
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), TestDataGenerator.CreateRandomBytes());


        _arrangeContext.RelationshipTemplates.Add(template);
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();

        var accessor = TestDataGenerator.CreateRandomAddress();


        // Act
        var result = _actContext
            .RelationshipTemplates
            .AsQueryable()
            .NotExpiredFor(accessor)
            .ToList();


        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void NotExpiredFor_FiltersOutExpiredTemplatesForNonParticipants()
    {
        // Arrange
        var templateCreator = TestDataGenerator.CreateRandomAddress();
        var requestCreator = TestDataGenerator.CreateRandomAddress();

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, Yesterday, TestDataGenerator.CreateRandomBytes());
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), TestDataGenerator.CreateRandomBytes());


        _arrangeContext.RelationshipTemplates.Add(template);
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();

        var accessor = TestDataGenerator.CreateRandomAddress();


        // Act
        var result = _actContext
            .RelationshipTemplates
            .AsQueryable()
            .NotExpiredFor(accessor)
            .ToList();


        // Assert
        result.Should().HaveCount(0);
    }
}
