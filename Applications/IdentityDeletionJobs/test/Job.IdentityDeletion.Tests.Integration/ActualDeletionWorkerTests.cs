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
    public async Task Logs_that_data_was_deleted()
    {
        // Arrange
        var identity = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<DevicesDbContext>();

        var auditLogEntries = await assertionContext.IdentityDeletionProcessAuditLogs.Where(a => a.IdentityAddressHash == Hasher.HashUtf8(identity.Address)).ToListAsync();

        var auditLogEntriesForDeletedData = auditLogEntries.Where(e => e.MessageKey == MessageKey.DataDeleted).ToList();

        auditLogEntriesForDeletedData.Should().HaveCount(13);

        auditLogEntriesForDeletedData.Should().AllSatisfy(e =>
        {
            e.AdditionalData.Should().HaveCount(1);
            e.AdditionalData!.First().Key.Should().Be("aggregateType");
        });

        var deletedAggregates = auditLogEntriesForDeletedData.SelectMany(e => e.AdditionalData!.Values).ToList();
        deletedAggregates.Should().Contain("Challenges");
        deletedAggregates.Should().Contain("PnsRegistrations");
        deletedAggregates.Should().Contain("Identities");
        deletedAggregates.Should().Contain("Files");
        deletedAggregates.Should().Contain("Messages");
        deletedAggregates.Should().Contain("QuotaIdentities");
        deletedAggregates.Should().Contain("Relationships");
        deletedAggregates.Should().Contain("RelationshipTemplates");
        deletedAggregates.Should().Contain("RelationshipTemplateAllocations");
        deletedAggregates.Should().Contain("ExternalEvents");
        deletedAggregates.Should().Contain("SyncRuns");
        deletedAggregates.Should().Contain("Datawallets");
        deletedAggregates.Should().Contain("Tokens");
    }

    [Fact]
    public async Task Deletes_the_identity()
    {
        // Arrange
        var identity = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<DevicesDbContext>();

        var identityAfterAct = await assertionContext.Identities.FirstOrDefaultAsync(i => i.Address == identity.Address);
        identityAfterAct.Should().BeNull();
    }

    [Fact]
    public async Task Anonymizes_the_identity_in_all_messages_instead_of_deleting_the_message()
    {
        // Arrange
        var identityToBeDeleted = await SeedDatabaseWithIdentityWithRipeDeletionProcess();
        var peer = CreateRandomIdentityAddress();

        GetService<MessagesDbContext>();

        var sentMessage = await SeedDatabaseWithMessage(from: identityToBeDeleted.Address, to: peer);
        var receivedMessage = await SeedDatabaseWithMessage(from: peer, to: identityToBeDeleted.Address);

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<MessagesDbContext>();

        var messagesOfIdentityAfterAct = await assertionContext.Messages.Where(Message.HasParticipant(identityToBeDeleted.Address)).ToListAsync();
        messagesOfIdentityAfterAct.Should().BeEmpty();

        var sentMessageAfterAct = await assertionContext.Messages.FirstOrDefaultAsync(m => m.Id == sentMessage.Id);
        sentMessageAfterAct.Should().NotBeNull();

        var receivedMessageAfterAct = await assertionContext.Messages.FirstOrDefaultAsync(m => m.Id == receivedMessage.Id);
        receivedMessageAfterAct.Should().NotBeNull();
    }

    [Fact]
    public async Task Deletes_relationships()
    {
        // Arrange
        var identityToBeDeleted = await SeedDatabaseWithIdentityWithRipeDeletionProcess();
        var peer = CreateRandomIdentityAddress();

        GetService<RelationshipsDbContext>();

        await SeedDatabaseWithActiveRelationshipBetween(peer, identityToBeDeleted.Address);

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<RelationshipsDbContext>();

        var relationshipsAfterAct = await assertionContext.Relationships.Where(Relationship.HasParticipant(identityToBeDeleted.Address)).ToListAsync();
        relationshipsAfterAct.Should().BeEmpty();
    }

    [Fact]
    public async Task Deletes_relationship_templates()
    {
        // Arrange
        var identityToBeDeleted = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        GetService<RelationshipsDbContext>();

        await SeedDatabaseWithRelationshipTemplateOf(identityToBeDeleted.Address);

        // Act
        await _host.StartAsync();

        // Assert
        var assertionContext = GetService<RelationshipsDbContext>();

        var templatesAfterAct = await assertionContext.RelationshipTemplates.Where(rt => rt.CreatedBy == identityToBeDeleted.Address).ToListAsync();
        templatesAfterAct.Should().BeEmpty();
    }

    #region Seeders

    private async Task<Message> SeedDatabaseWithMessage(IdentityAddress from, IdentityAddress to)
    {
        var dbContext = GetService<MessagesDbContext>();

        var recipient = new RecipientInformation(to, RelationshipId.New(), []);
        var message = new Message(from, DeviceId.New(), [], [], [recipient]);

        await dbContext.SaveEntity(message);

        return message;
    }

    private async Task SeedDatabaseWithActiveRelationshipBetween(IdentityAddress from, IdentityAddress to)
    {
        var dbContext = GetService<RelationshipsDbContext>();

        var template = new RelationshipTemplate(from, DeviceId.New(), null, null, []);
        var relationship = new Relationship(template, to, DeviceId.New(), [], []);
        relationship.Accept(from, DeviceId.New(), []);

        await dbContext.SaveEntity(relationship);
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

        var identity = new Identity("test", CreateRandomIdentityAddress(), [], tier!.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        identity.Devices.Add(device);

        SystemTime.Set(SystemTime.UtcNow.AddMonths(-1));
        identity.StartDeletionProcessAsOwner(device.Id);
        SystemTime.Reset();

        await dbContext.SaveEntity(identity);

        return identity;
    }

    #endregion

    private T GetService<T>() where T : notnull
    {
        return _host.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }
}
