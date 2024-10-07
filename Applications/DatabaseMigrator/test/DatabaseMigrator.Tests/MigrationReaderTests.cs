using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Backbone.DatabaseMigrator.Tests;

public class MigrationReaderTests
{
    [Fact]
    public async Task Returns_migrations_in_correct_order()
    {
        // Arrange
        var migrationReader = await CreateMigrationReader();

        // Act
        var migrations = await migrationReader.ReadMigrations();

        // Assert
        var migrationNames = migrations.Select(m => m.Name);

        migrationNames.Should().ContainInOrder(
            "20240701073944_Init",
            "20240701074627_Init",
            "20240701074820_Init",
            "20240701075857_Init",
            "20240701081741_Init",
            "20240701082135_Init",
            "20240701075023_Init",
            "20240701075245_Init",
            "20240701060320_Init",
            "20240703093047_RemoveRelationshipId",
            "20240703100000_ConfigureRelationshipAuditLogDeleteBehavior",
            "20240708114348_AddAdditionalDataToIdentityDeletionProcessAuditLogEntry",
            "20240710125429_AddIsRelationshipDecomposedByRecipientAndIsRelationshipDecomposedBySenderProperties",
            "20240807121033_PersonalizedTokens",
            "20240830112624_RemoveBlobReferenceColumn",
            "20240830113359_RemoveDeletedAtPropertyFromRelationshipTemplate",
            "20240830164312_HashIndexesForIds",
            "20240830164612_HashIndexForMessageCreatedByField",
            "20240830164658_HashIndexesForRelationshipIdentityAddresses",
            "20240902141902_MakeIdentityDeletionProcessDeletionStartedAtPropertyNullable",
            "20240904100328_IncreaseMaxSizeOfSyncErrorErrorCodeTo100",
            "20240906075221_PersonalizedRelationshipTemplates",
            "20240909071633_AddUniqueIndexOnActiveDeletionProcesses"
        );
    }

    private static async Task<MigrationReader> CreateMigrationReader()
    {
        await StartPostgres("postgres", "admin", 5444);

        var services = new ServiceCollection();

        services.AddSingleton<IEventBus, DummyEventBus>();
        services.AddDatabase(new SqlDatabaseConfiguration { Provider = "Postgres", ConnectionString = "User ID=postgres;Password=admin;Server=localhost;Port=5444;Database=enmeshed;" });
        services.AddSingleton<DbContextProvider>();
        services.AddSingleton<MigrationReader>();

        var dbContextProvider = new DbContextProvider(services.BuildServiceProvider());
        var migrationReader = new MigrationReader(dbContextProvider);
        return migrationReader;
    }

    private static async Task StartPostgres(string username, string password, int port)
    {
        var postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername(username)
            .WithPassword(password)
            .WithPortBinding(port, 5432)
            .Build();

        await postgreSqlContainer.StartAsync();
    }
}
