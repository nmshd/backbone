using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.Shouldly.Extensions;
using Backbone.UnitTestTools.TestDoubles.Fakes;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public class RelationshipTemplateQueryableExtensionsTests : AbstractTestsBase
{
    private static readonly DateTime YESTERDAY = DateTime.UtcNow.AddDays(-1);

    private readonly RelationshipsDbContext _arrangeContext;
    private readonly RelationshipsDbContext _actContext;

    public RelationshipTemplateQueryableExtensionsTests()
    {
        (_arrangeContext, _actContext, _) = FakeDbContextFactory.CreateDbContexts<RelationshipsDbContext>();
    }

    [Fact]
    public void NotExpiredFor_DoesNotFilterOutTemplatesForParticipants()
    {
        // Arrange
        var templateCreator = CreateRandomIdentityAddress();
        var requestCreator = CreateRandomIdentityAddress();

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, YESTERDAY, new byte[2]);
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), new byte[2], []);

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
        result.ShouldHaveCount(1);
    }

    [Fact]
    public void NotExpiredFor_DoesNotFilterOutTemplatesWithoutExpiryDate()
    {
        // Arrange
        var templateCreator = CreateRandomIdentityAddress();
        var requestCreator = CreateRandomIdentityAddress();

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, null, CreateRandomBytes());
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), CreateRandomBytes(), []);

        _arrangeContext.RelationshipTemplates.Add(template);
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();

        var accessor = CreateRandomIdentityAddress();


        // Act
        var result = _actContext
            .RelationshipTemplates
            .AsQueryable()
            .NotExpiredFor(accessor)
            .ToList();


        // Assert
        result.ShouldHaveCount(1);
    }

    [Fact]
    public void NotExpiredFor_FiltersOutExpiredTemplatesForNonParticipants()
    {
        // Arrange
        var templateCreator = CreateRandomIdentityAddress();
        var requestCreator = CreateRandomIdentityAddress();

        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, YESTERDAY, CreateRandomBytes());
        var relationship = new Relationship(template, requestCreator, DeviceId.New(), CreateRandomBytes(), []);


        _arrangeContext.RelationshipTemplates.Add(template);
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();

        var accessor = CreateRandomIdentityAddress();


        // Act
        var result = _actContext
            .RelationshipTemplates
            .AsQueryable()
            .NotExpiredFor(accessor)
            .ToList();


        // Assert
        result.ShouldHaveCount(0);
    }
}
