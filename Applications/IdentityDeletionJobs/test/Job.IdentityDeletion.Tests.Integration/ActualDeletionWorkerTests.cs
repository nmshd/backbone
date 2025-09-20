using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Relationship = Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship;

namespace Backbone.Job.IdentityDeletion.Tests.Integration;

public class ActualDeletionWorkerTests : AbstractTestsBase
{
    private readonly IHost _host;
    private readonly ITestOutputHelper _testOutputHelper;

    public ActualDeletionWorkerTests(ITestOutputHelper testOutputHelper)
    {
        var hostBuilder = Program.CreateHostBuilder(["--Worker", "ActualDeletionWorker"]);
        _host = hostBuilder.Build();
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Logs_that_data_was_deleted()
    {
        // Arrange
        var identity = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        // Act
        await _host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var assertionContext = GetService<DevicesDbContext>();

        var auditLogEntries = await assertionContext.IdentityDeletionProcessAuditLogs.Where(a => a.IdentityAddressHash == Hasher.HashUtf8(identity.Address))
            .ToListAsync(TestContext.Current.CancellationToken);

        var auditLogEntriesForDeletedData = auditLogEntries.Where(e => e.MessageKey == MessageKey.DataDeleted).ToList();

        auditLogEntriesForDeletedData.ShouldHaveCount(13);

        auditLogEntriesForDeletedData.ShouldAllBe(e =>
            e.AdditionalData != null && e.AdditionalData.Count == 1 && e.AdditionalData.First().Key == "aggregateType"
        );

        var deletedAggregates = auditLogEntriesForDeletedData.SelectMany(e => e.AdditionalData!.Values).ToList();
        deletedAggregates.ShouldContain("Challenges");
        deletedAggregates.ShouldContain("PnsRegistrations");
        deletedAggregates.ShouldContain("Identities");
        deletedAggregates.ShouldContain("Files");
        deletedAggregates.ShouldContain("QuotaIdentities");
        deletedAggregates.ShouldContain("Relationships");
        deletedAggregates.ShouldContain("RelationshipTemplates");
        deletedAggregates.ShouldContain("RelationshipTemplateAllocations");
        deletedAggregates.ShouldContain("ExternalEvents");
        deletedAggregates.ShouldContain("SyncRuns");
        deletedAggregates.ShouldContain("Datawallets");
        deletedAggregates.ShouldContain("Tokens");
        deletedAggregates.ShouldContain("AnnouncementRecipients");
    }

    [Fact]
    public async Task Deletes_the_identity_when_it_is_in_status_ToBeDeleted()
    {
        // Arrange
        var identity = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        // Act
        await _host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var assertionContext = GetService<DevicesDbContext>();

        var identityAfterAct = await assertionContext.Identities.FirstOrDefaultAsync(i => i.Address == identity.Address, TestContext.Current.CancellationToken);
        identityAfterAct.ShouldBeNull();
    }

    [Fact]
    public async Task Deletes_the_identity_when_it_is_in_status_Deleting()
    {
        // Arrange
        var identity = await SeedDatabaseWithIdentityInStatusDeleting();

        // Act
        await _host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var assertionContext = GetService<DevicesDbContext>();

        var identityAfterAct = await assertionContext.Identities.FirstOrDefaultAsync(i => i.Address == identity.Address, TestContext.Current.CancellationToken);
        identityAfterAct.ShouldBeNull();
    }

    /*[Fact]
    public async Task Deletes_relationships() //TODO: Check deadlock
    {
        // Arrange
        var identityToBeDeleted = await LogTime(SeedDatabaseWithIdentityWithRipeDeletionProcess(), "Seed DB with identity with ripe deletion process");
        var peerOfIdentityToBeDeleted = await LogTime(SeedDatabaseWithIdentity(), "Seed DB with identity");

        await LogTime(SeedDatabaseWithActiveRelationshipBetween(identityToBeDeleted, peerOfIdentityToBeDeleted), "Seed DB with relationship");

        // Act
        await LogTime(_host.StartAsync(TestContext.Current.CancellationToken), "Run Deletion Job");

        // Assert
        var assertionContext = GetService<RelationshipsDbContext>();

        var relationshipsAfterAct = await LogTime(assertionContext.Relationships.Where(Relationship.HasParticipant(identityToBeDeleted.Address)).ToListAsync(TestContext.Current.CancellationToken),
            "Get relationships");
        relationshipsAfterAct.ShouldBeEmpty();
    }*/

    [Fact]
    public async Task Deletes_relationship_templates()
    {
        // Arrange
        var identityToBeDeleted = await SeedDatabaseWithIdentityWithRipeDeletionProcess();

        await SeedDatabaseWithRelationshipTemplateOf(identityToBeDeleted.Address);

        // Act
        await _host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var assertionContext = GetService<RelationshipsDbContext>();

        var templatesAfterAct = await assertionContext.RelationshipTemplates.Where(rt => rt.CreatedBy == identityToBeDeleted.Address).ToListAsync(TestContext.Current.CancellationToken);
        templatesAfterAct.ShouldBeEmpty();
    }

    [Fact]
    public async Task Verifies_deletion()
    {
        // Arrange
        var identity = await LogTime(SeedDatabaseWithIdentity(), "Create Identity");
        var verifier = GetService<IDeletionVerifier>();

        // Act
        var result = await LogTime(verifier.VerifyDeletion([identity.Address.Value], TestContext.Current.CancellationToken), "Verify Identity");

        // Await
        result.Success.ShouldBeFalse();
    }

    private T GetService<T>() where T : notnull
    {
        return _host.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    #region Seeders

    private async Task SeedDatabaseWithActiveRelationshipBetween(Identity from, Identity to)
    {
        var dbContext = GetService<RelationshipsDbContext>();

        var template = new RelationshipTemplate(to.Address, to.Devices.First().Id, null, null, []);
        var relationship = new Relationship(template, from.Address, from.Devices.First().Id, [], []);
        relationship.Accept(to.Address, to.Devices.First().Id, []);

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

        var identity = new Identity("test", CreateRandomIdentityAddress(), [], tier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        var device = identity.Devices.First();

        SystemTime.Set(SystemTime.UtcNow.AddMonths(-1));
        identity.StartDeletionProcess(device.Id);
        SystemTime.Reset();

        await dbContext.SaveEntity(identity);

        return identity;
    }

    private async Task<Identity> SeedDatabaseWithIdentityInStatusDeleting()
    {
        var dbContext = GetService<DevicesDbContext>();

        var tier = await dbContext.Tiers.FirstAsync(t => t.Id != Tier.QUEUED_FOR_DELETION.Id);

        var identity = new Identity("test", CreateRandomIdentityAddress(), [], tier.Id, 1, CommunicationLanguage.DEFAULT_LANGUAGE);

        var device = identity.Devices.First();

        identity.StartDeletionProcess(device.Id, 0);
        identity.DeletionStarted();

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

    //Test method
    private async Task<T> LogTime<T>(Task<T> task, string hint = "")
    {
        _testOutputHelper.WriteLine($"Starting \"{hint}\"");
        var start = DateTime.UtcNow;

        var result = await task;

        var duration = DateTime.UtcNow - start;
        _testOutputHelper.WriteLine($"Completed \"{hint}\" (took {duration.TotalSeconds} s)");

        return result;
    }

    private async Task LogTime(Task task, string hint = "")
    {
        await Console.Error.WriteLineAsync($"Starting \"{hint}\"");
        var start = DateTime.UtcNow;

        await task;

        var duration = DateTime.UtcNow - start;
        await Console.Error.WriteLineAsync($"Completed \"{hint}\" (took {duration.TotalSeconds} s)");
    }

    #endregion
}
