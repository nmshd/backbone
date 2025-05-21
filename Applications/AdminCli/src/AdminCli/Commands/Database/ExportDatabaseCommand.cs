using System.CommandLine;
using System.IO.Compression;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.AdminCli.Commands.Database.Types;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminCli.Commands.Database;

public class ExportDatabaseCommand : AdminCliCommand
{
    private readonly DevicesDbContext _devicesDbContext;
    private readonly RelationshipsDbContext _relationshipsDbContext;
    private readonly FilesDbContext _filesDbContext;
    private readonly MessagesDbContext _messagesDbContext;
    private readonly SynchronizationDbContext _synchronizationDbContext;
    private readonly TokensDbContext _tokensDbContext;

    private readonly string _pathToExportDirectory = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone", "export");
    private readonly string _pathToZipFile = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone", $"export-{SystemTime.UtcNow:yyyyMMdd_HHmmss}.zip");

    private Dictionary<string, string?> _addressToClientDisplayName = null!;


    public ExportDatabaseCommand(IMediator mediator, DevicesDbContext devicesDbContext, RelationshipsDbContext relationshipsDbContext, FilesDbContext filesDbContext,
        MessagesDbContext messagesDbContext, SynchronizationDbContext synchronizationDbContext, TokensDbContext tokensDbContext)
        : base(mediator, "export", "Create a zip file with the most important database tables exported as csv.")
    {
        _devicesDbContext = devicesDbContext;
        _relationshipsDbContext = relationshipsDbContext;
        _filesDbContext = filesDbContext;
        _messagesDbContext = messagesDbContext;
        _synchronizationDbContext = synchronizationDbContext;
        _tokensDbContext = tokensDbContext;

        this.SetHandler(ExportDatabase);

        DeleteExportDirectory();
        DeleteOldZipFiles();
        CreateExportDirectory();
    }

    private void DeleteExportDirectory()
    {
        if (Directory.Exists(_pathToExportDirectory))
        {
            Directory.Delete(_pathToExportDirectory, true);
        }
    }

    private void DeleteOldZipFiles()
    {
        var directory = Path.GetDirectoryName(_pathToZipFile)!;

        if (!Directory.Exists(directory))
            return;

        var oldExportZipFiles = Directory.GetFiles(directory, "export-*.zip", SearchOption.TopDirectoryOnly);
        foreach (var file in oldExportZipFiles)
        {
            File.Delete(file);
        }
    }

    private void CreateExportDirectory()
    {
        if (!Directory.Exists(_pathToExportDirectory))
        {
            Directory.CreateDirectory(_pathToExportDirectory);
        }
    }

    private async Task ExportDatabase()
    {
        _addressToClientDisplayName = await _devicesDbContext.Identities.ToDictionaryAsync(
            i => i.Address.ToString(),
            i => _devicesDbContext.Set<CustomOpenIddictEntityFrameworkCoreApplication>().FirstOrDefault(c => c.ClientId == i.ClientId)?.DisplayName
        );

        await ExportDevices();
        await ExportRelationshipTemplates();
        await ExportRelationships();
        await ExportFiles();
        await ExportMessages();
        await ExportDatawalletModifications();
        await ExportTokens();
        await ExportSyncErrors();
        await ExportDeletionAuditLogItems();

        ZipExportDirectory();

        DeleteExportDirectory();

        Console.WriteLine(
            $"""
             Export completed. The zip file was saved at the following location: {_pathToZipFile}. If you ran this command from Kubernetes, you can copy it to your local system by running the following command:
             kubectl cp <namespace-name><pod-name>:{_pathToZipFile} <local-destination-path>
             """);
    }

    private async Task ExportDevices()
    {
        var devices = _devicesDbContext
            .Devices
            .Select(d => new DeviceExport
            {
                DeviceId = d.Id.Value,
                LastLoginAt = d.User.LastLoginAt,
                IdentityAddress = d.Identity.Address.Value,
                CreatedAt = d.CreatedAt,
                Tier = _devicesDbContext.Tiers.FirstOrDefault(t => t.Id == d.Identity.TierId)!.Name.Value,
                IdentityStatus = d.Identity.Status,
                IdentityDeletionGracePeriodEndsAt = d.Identity.DeletionGracePeriodEndsAt,
                Platform = _devicesDbContext.PnsRegistrations.Any(r => r.DeviceId == d.Id)
                    ? _devicesDbContext.PnsRegistrations.First(r => r.DeviceId == d.Id)
                        .Handle.Platform
                    : null
            })
            .ToAsyncEnumerable();

        await StreamToCSV(devices, "devices.csv", d => { d.ClientName = GetClientName(d.IdentityAddress); });
    }

    private async Task ExportDeletionAuditLogItems()
    {
        var deletionAuditLogItems = _devicesDbContext
            .IdentityDeletionProcessAuditLogs
            .Select(i => new DeletionAuditLogItemExport
            {
                OldStatus = i.OldStatus,
                CreatedAt = i.CreatedAt,
                NewStatus = i.NewStatus,
                IdentityAddressHash = i.IdentityAddressHash,
                MessageKey = i.MessageKey
            })
            .ToAsyncEnumerable();

        await StreamToCSV(deletionAuditLogItems, "deletionAuditLogItems.csv", _ => { });
    }

    private async Task ExportRelationshipTemplates()
    {
        var templates = _relationshipsDbContext
            .RelationshipTemplates
            .Select(t => new RelationshipTemplateExport
            {
                TemplateId = t.Id.Value,
                CreatedBy = t.CreatedBy.Value,
                CreatedAt = t.CreatedAt,
                AllocatedAt = t.Allocations.Any()
                    ? t.Allocations.First()
                        .AllocatedAt
                    : null
            })
            .ToAsyncEnumerable();

        await StreamToCSV(templates, "relationshipTemplates.csv", t => t.CreatedByClientName = GetClientName(t.CreatedBy));
    }

    private async Task ExportRelationships()
    {
        var relationships = _relationshipsDbContext
            .Relationships
            .Select(r => new RelationshipExport
            {
                RelationshipId = r.Id.Value,
                TemplateId = r.RelationshipTemplateId == null ? null : r.RelationshipTemplateId.Value,
                From = r.From.Value,
                To = r.To.Value,
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                FromHasDecomposed = r.FromHasDecomposed,
                ToHasDecomposed = r.ToHasDecomposed,
                TemplateCreatedAt = r.RelationshipTemplate == null ? null : r.RelationshipTemplate.CreatedAt
            })
            .ToAsyncEnumerable();

        await StreamToCSV(relationships, "relationships.csv", r =>
        {
            r.FromClientName = GetClientName(r.From);
            r.ToClientName = GetClientName(r.To);
        });
    }

    private async Task ExportFiles()
    {
        var files = _filesDbContext
            .FileMetadata
            .Select(f => new FileExport
            {
                FileId = f.Id,
                CreatedBy = f.CreatedBy.Value,
                CreatedAt = f.CreatedAt,
                Owner = f.Owner.Value,
                CipherSize = f.CipherSize,
                ExpiresAt = f.ExpiresAt,
            })
            .ToAsyncEnumerable();

        await StreamToCSV(files, "files.csv", f =>
        {
            f.CreatedByClientName = GetClientName(f.CreatedBy);
            f.OwnerClientName = GetClientName(f.Owner);
        });
    }

    private async Task ExportMessages()
    {
        var messages = _messagesDbContext
            .Messages
            .Select(m => new MessageExport
            {
                MessageId = m.Id.Value,
                CreatedBy = m.CreatedBy.Value,
                RelationshipId = m.Recipients.First().RelationshipId,
                Recipient = m.Recipients.First().Address.Value,
                CreatedAt = m.CreatedAt,
                ReceivedAt = m.Recipients.First().ReceivedAt,
                CipherSize = m.Body.Length,
            })
            .ToAsyncEnumerable();

        await StreamToCSV(messages, "messages.csv", m =>
        {
            m.CreatedByClientName = GetClientName(m.CreatedBy);
            m.RecipientClientName = GetClientName(m.Recipient);
        });
    }

    private async Task ExportDatawalletModifications()
    {
        var modifications = _synchronizationDbContext
            .DatawalletModifications
            .Select(m => new DatawalletModificationExport
            {
                DatawalletModificationId = m.Id,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy.Value,
                ObjectIdentifier = m.ObjectIdentifier,
                Collection = m.Collection,
                Type = m.Type,
                PayloadCategory = m.PayloadCategory,
                PayloadSize = m.EncryptedPayload == null ? null : m.EncryptedPayload.Length
            })
            .ToAsyncEnumerable();

        await StreamToCSV(modifications, "datawalletModifications.csv", m => { m.CreatedByClientName = GetClientName(m.CreatedBy); });
    }

    private async Task ExportSyncErrors()
    {
        var syncErrors = _synchronizationDbContext
            .SyncErrors
            .Select(e => new SyncErrorExport
            {
                ErrorId = e.Id.Value,
                SyncItemOwner = e.ExternalEvent.Owner.Value,
                CreatedAt = e.SyncRun.FinalizedAt,
                ErrorCode = e.ErrorCode
            })
            .ToAsyncEnumerable();

        await StreamToCSV(syncErrors, "syncErrors.csv", m => { m.SyncItemOwnerClientName = GetClientName(m.SyncItemOwner); });
    }

    private async Task ExportTokens()
    {
        var modifications = _tokensDbContext
            .Tokens
            .Select(t => new TokenExport
            {
                TokenId = t.Id,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy.Value,
                CipherSize = t.Content.Length,
                ExpiresAt = t.ExpiresAt
            })
            .ToAsyncEnumerable();

        await StreamToCSV(modifications, "tokens.csv", m => { m.CreatedByClientName = GetClientName(m.CreatedBy); });
    }

    private string? GetClientName(string address)
    {
        return _addressToClientDisplayName.TryGetValue(address, out var clientName) ? clientName : null;
    }

    private async Task StreamToCSV<T>(IAsyncEnumerable<T> objects, string filename, Action<T> enricher) where T : notnull
    {
        await using var outputFileStream = new StreamWriter(Path.Join(_pathToExportDirectory, filename), append: false);

        var headerLine = string.Join(",", typeof(T).GetProperties().Select(p => p.Name));
        await outputFileStream.WriteLineAsync(headerLine);

        await foreach (var obj in objects)
        {
            enricher(obj);
            await outputFileStream.WriteLineAsync(obj.ToCsv());
        }
    }

    private void ZipExportDirectory()
    {
        if (File.Exists(_pathToZipFile))
            File.Delete(_pathToZipFile);

        ZipFile.CreateFromDirectory(_pathToExportDirectory, _pathToZipFile);
    }
}
