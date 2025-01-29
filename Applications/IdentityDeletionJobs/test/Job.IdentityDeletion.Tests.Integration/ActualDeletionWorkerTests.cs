using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Relationship = Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship;

namespace Backbone.Job.IdentityDeletion.Tests.Integration;

public class ActualDeletionWorkerTests : AbstractTestsBase
{
    private readonly IHost _host;

    public ActualDeletionWorkerTests()
    {
        var hostBuilder = Program.CreateHostBuilder(["--Worker", "ActualDeletionWorker"]);

        _host = hostBuilder.Build();
    }

    [Fact]
    public async Task Deletes_relationships()
    {
        // Arrange
        var identityToBeDeleted = await SeedDatabaseWithIdentityWithRipeDeletionProcess();
        var peerOfIdentityToBeDeleted = await SeedDatabaseWithIdentity();

        await SeedDatabaseWithActiveRelationshipBetween(identityToBeDeleted, peerOfIdentityToBeDeleted);

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<RelationshipsDbContext>();

        var relationshipsAfterAct = await assertionContext.Relationships.Where(Relationship.HasParticipant(identityToBeDeleted.Address)).ToListAsync();
        relationshipsAfterAct.Should().BeEmpty();
    }

    private T GetService<T>() where T : notnull
    {
        return _host.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    #region Seeders

    private async Task<Message> SeedDatabaseWithMessage(Relationship relationship, Identity from, Identity to)
    {
        var dbContext = GetService<MessagesDbContext>();

        var recipient = new RecipientInformation(to.Address, RelationshipId.Parse(relationship.Id.Value), []);
        var message = new Message(from.Address, from.Devices.First().Id, [], [], [recipient]);

        await dbContext.SaveEntity(message);

        return message;
    }

    private async Task<Relationship> SeedDatabaseWithActiveRelationshipBetween(Identity from, Identity to)
    {
        var dbContext = GetService<RelationshipsDbContext>();

        var template = new RelationshipTemplate(to.Address, to.Devices.First().Id, null, null, []);
        var relationship = new Relationship(template, from.Address, from.Devices.First().Id, [], []);
        relationship.Accept(to.Address, to.Devices.First().Id, []);

        await dbContext.SaveEntity(relationship);

        return relationship;
    }

    private async Task SeedDatabaseWithRelationshipTemplateOf(IdentityAddress owner)
    {
        var dbContext = GetService<RelationshipsDbContext>();

        var template = new RelationshipTemplate(owner, DeviceId.New(), null, null, []);

        await dbContext.SaveEntity(template);
    }

    private async Task<Identity> SeedDatabaseWithIdentityWithRipeDeletionProcess()
    {
        var dbContext = GetService<DevicesDbContext>();

        var tier = await dbContext.Tiers.FirstAsync(t => t.Id != Tier.QUEUED_FOR_DELETION.Id);

        var identity = new Identity("test", CreateRandomIdentityAddress(), [], tier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        var device = identity.Devices.First();

        SystemTime.Set(SystemTime.UtcNow.AddMonths(-1));
        identity.StartDeletionProcessAsOwner(device.Id);
        SystemTime.Reset();

        await dbContext.SaveEntity(identity);

        return identity;
    }

    private async Task<Identity> SeedDatabaseWithIdentity()
    {
        var dbContext = GetService<DevicesDbContext>();

        var tier = await dbContext.Tiers.FirstAsync(t => t.Id != Tier.QUEUED_FOR_DELETION.Id);

        var identity = new Identity("test", CreateRandomIdentityAddress(), [], tier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        await dbContext.SaveEntity(identity);

        return identity;
    }

    #endregion
}
