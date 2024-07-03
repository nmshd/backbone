using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Relationship = Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship;

namespace Backbone.Job.IdentityDeletion.Tests.Integration;

public class ActualDeletionWorkerTests
{
    private readonly IHost _host;

    public ActualDeletionWorkerTests()
    {
        var hostBuilder = Program.CreateHostBuilder(["--Worker", "ActualDeletionWorker"]);

        _host = hostBuilder.Build();

        GetService<RelationshipsDbContext>().Database.Migrate();
        GetService<MessagesDbContext>().Database.Migrate();
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
        auditLogEntries.Should().HaveCount(14);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.ChallengesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.PnsRegistrationsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.IdentitiesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.FilesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.MessagesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.QuotaIdentitiesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.RelationshipsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.RelationshipTemplatesDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.RelationshipTemplateAllocationsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.ExternalEventsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.SyncRunsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.DatawalletsDeleted);
        auditLogEntries.Should().Contain(a => a.MessageKey == MessageKey.TokensDeleted);
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
        var peer = TestDataGenerator.CreateRandomIdentityAddress();

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
        var peer = TestDataGenerator.CreateRandomIdentityAddress();

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

    private async Task<Message> SeedDatabaseWithMessage(IdentityAddress from, IdentityAddress to)
    {
        var dbContext = GetService<MessagesDbContext>();

        var recipient = new RecipientInformation(to, []);

        var message = new Message(from, DeviceId.New(), [], [], [recipient]);

        await dbContext.SaveEntity(message);

        return message;
    }

    private async Task SeedDatabaseWithActiveRelationshipBetween(IdentityAddress from, IdentityAddress to)
    {
        var template = new RelationshipTemplate(from, DeviceId.New(), null, null, []);
        var relationship = new Relationship(template, to, DeviceId.New(), [], []);
        relationship.Accept(from, DeviceId.New(), []);
        var dbContext = GetService<RelationshipsDbContext>();
        await dbContext.SaveEntity(relationship);
    }

    private async Task SeedDatabaseWithRelationshipTemplateOf(IdentityAddress owner)
    {
        var template = new RelationshipTemplate(owner, DeviceId.New(), null, null, []);
        var dbContext = GetService<RelationshipsDbContext>();
        await dbContext.SaveEntity(template);
    }

    private async Task<Identity> SeedDatabaseWithIdentityWithRipeDeletionProcess()
    {
        var dbContext = GetService<DevicesDbContext>();

        var identity = new Identity("test", TestDataGenerator.CreateRandomIdentityAddress(), [], TierId.Generate(), 1);

        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        identity.Devices.Add(device);

        SystemTime.Set(SystemTime.UtcNow.AddMonths(-1));
        identity.StartDeletionProcessAsOwner(device.Id);
        SystemTime.Reset();

        await dbContext.SaveEntity(identity);

        return identity;
    }

    private T GetService<T>() where T : notnull
    {
        return _host.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }
}
