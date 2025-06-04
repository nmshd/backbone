using System.CommandLine;
using System.IO.Compression;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.AdminCli.Commands.Database.Model;
using Backbone.Tooling;
using MediatR;
using Spectre.Console;

namespace Backbone.AdminCli.Commands.Database;

public class ExportDatabaseCommand : AdminCliCommand
{
    private readonly AdminApiDbContext _adminApiDbContext;

    private readonly string _pathToExportDirectory = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone", "export");
    private readonly string _pathToZipFile = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone", $"export-{SystemTime.UtcNow:yyyyMMdd_HHmmss}.zip");

    public ExportDatabaseCommand(IMediator mediator, AdminApiDbContext adminApiDbContext)
        : base(mediator, "export", "Create a zip file with the most important database tables exported as csv.")
    {
        _adminApiDbContext = adminApiDbContext;

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
        await AnsiConsole.Progress()
            .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new SpinnerColumn(), new PercentageColumn())
            .StartAsync(async ctx =>
            {
                var exportDevicesTask = ctx.AddTask("Devices", autoStart: false);
                var exportDeletionAuditLogItemsTask = ctx.AddTask("Deletion audit log items", autoStart: false);
                var exportRelationshipTemplatesTask = ctx.AddTask("Relationship templates", autoStart: false);
                var exportRelationshipsTask = ctx.AddTask("Relationships", autoStart: false);
                var exportFilesTask = ctx.AddTask("Files", autoStart: false);
                var exportMessagesTask = ctx.AddTask("Messages", autoStart: false);
                var exportDatawalletModificationsTask = ctx.AddTask("Datawallet modifications", autoStart: false);
                var exportTokensTask = ctx.AddTask("Tokens", autoStart: false);
                var exportSyncErrorsTask = ctx.AddTask("Sync errors", autoStart: false);

                exportDevicesTask.StartTask();
                await ExportDevices(exportDevicesTask);

                exportDeletionAuditLogItemsTask.StartTask();
                await ExportDeletionAuditLogItems(exportDeletionAuditLogItemsTask);

                exportRelationshipTemplatesTask.StartTask();
                await ExportRelationshipTemplates(exportRelationshipTemplatesTask);

                exportRelationshipsTask.StartTask();
                await ExportRelationships(exportRelationshipsTask);

                exportFilesTask.StartTask();
                await ExportFiles(exportFilesTask);

                exportMessagesTask.StartTask();
                await ExportMessages(exportMessagesTask);

                exportDatawalletModificationsTask.StartTask();
                await ExportDatawalletModifications(exportDatawalletModificationsTask);

                exportTokensTask.StartTask();
                await ExportTokens(exportTokensTask);

                exportSyncErrorsTask.StartTask();
                await ExportSyncErrors(exportSyncErrorsTask);
            });

        ZipExportDirectory();

        DeleteExportDirectory();
        Console.WriteLine(
            $"""
             Export completed. The zip file was saved at the following location: {_pathToZipFile}. If you ran this command from Kubernetes, you can copy it to your local system by running the following command:
             kubectl cp <namespace-name><pod-name>:{_pathToZipFile} <local-destination-path>
             """);
    }

    private async Task ExportDevices(ProgressTask progressReporter)
    {
        var devices = _adminApiDbContext
            .Devices
            .Select(d => new DeviceExport
            {
                DeviceId = d.Id,
                LastLoginAt = d.User.LastLoginAt,
                IdentityAddress = d.Identity.Address,
                CreatedAt = d.CreatedAt,
                Tier = _adminApiDbContext.Tiers.FirstOrDefault(t => t.Id == d.Identity.TierId)!.Name,
                IdentityStatus = d.Identity.Status,
                IdentityDeletionGracePeriodEndsAt = d.Identity.DeletionGracePeriodEndsAt,
                Platform = _adminApiDbContext.PnsRegistrations.Any(r => r.DeviceId == d.Id)
                    ? _adminApiDbContext.PnsRegistrations.First(r => r.DeviceId == d.Id)
                        .Handle.Platform
                    : null,
                ClientName = _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == d.Identity.ClientId) == null
                    ? null
                    : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == d.Identity.ClientId)!.DisplayName,
                ClientId = d.Identity.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(devices, "devices.csv", progressReporter);
    }

    private async Task ExportDeletionAuditLogItems(ProgressTask progressReporter)
    {
        var deletionAuditLogItems = _adminApiDbContext
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

        await StreamToCSV(deletionAuditLogItems, "deletionAuditLogItems.csv", progressReporter);
    }

    private async Task ExportRelationshipTemplates(ProgressTask progressReporter)
    {
        var templates = _adminApiDbContext
            .RelationshipTemplates
            .Select(t => new RelationshipTemplateExport
            {
                TemplateId = t.Id,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                AllocatedAt = t.Allocations.Any()
                    ? t.Allocations.First()
                        .AllocatedAt
                    : null,
                CreatedByClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(templates, "relationshipTemplates.csv", progressReporter);
    }

    private async Task ExportRelationships(ProgressTask progressReporter)
    {
        var relationships = _adminApiDbContext
            .Relationships
            .Select(r => new RelationshipExport
            {
                RelationshipId = r.Id,
                TemplateId = r.RelationshipTemplateId == null ? null : r.RelationshipTemplateId.ToString(),
                From = r.From,
                To = r.To,
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                FromHasDecomposed = r.FromHasDecomposed,
                ToHasDecomposed = r.ToHasDecomposed,
                TemplateCreatedAt = r.RelationshipTemplate == null ? null : r.RelationshipTemplate.CreatedAt,
                FromClientName = _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.From)!.ClientId) == null
                    ? null
                    : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.From)!.ClientId)!.DisplayName,
                FromClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.From)!.ClientId,
                ToClientName = _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.To)!.ClientId) == null
                    ? null
                    : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.To)!.ClientId)!.DisplayName,
                ToClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == r.To)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(relationships, "relationships.csv", progressReporter);
    }

    private async Task ExportFiles(ProgressTask progressReporter)
    {
        var files = _adminApiDbContext
            .Files
            .Select(f => new FileExport
            {
                FileId = f.Id,
                CreatedBy = f.CreatedBy,
                CreatedAt = f.CreatedAt,
                LastOwnershipClaimAt = f.LastOwnershipClaimAt,
                Owner = f.Owner,
                CipherSize = f.CipherSize,
                ExpiresAt = f.ExpiresAt,
                CreatedByClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.CreatedBy)!.ClientId,
                OwnerClientName = _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.Owner)!.ClientId) == null
                    ? null
                    : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.Owner)!.ClientId)!.DisplayName,
                OwnerClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == f.Owner)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(files, "files.csv", progressReporter);
    }

    private async Task ExportMessages(ProgressTask progressReporter)
    {
        var messages = _adminApiDbContext
            .Messages
            .Select(m => new MessageExport
            {
                MessageId = m.Id,
                CreatedBy = m.CreatedBy,
                RelationshipId = m.Recipients.First().RelationshipId,
                Recipient = m.Recipients.First().Address,
                CreatedAt = m.CreatedAt,
                ReceivedAt = m.Recipients.First().ReceivedAt,
                CipherSize = m.Body.Length,
                CreatedByClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId,
                RecipientClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a =>
                        a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.Recipients.First().Address)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a =>
                            a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.Recipients.First().Address)!.ClientId)!.DisplayName,
                RecipientClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.Recipients.First().Address)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(messages, "messages.csv", progressReporter);
    }

    private async Task ExportDatawalletModifications(ProgressTask progressReporter)
    {
        var modifications = _adminApiDbContext
            .DatawalletModifications
            .Select(m => new DatawalletModificationExport
            {
                DatawalletModificationId = m.Id,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy,
                ObjectIdentifier = m.ObjectIdentifier,
                Collection = m.Collection,
                Type = m.Type,
                PayloadCategory = m.PayloadCategory,
                PayloadSize = m.EncryptedPayload == null ? null : m.EncryptedPayload.Length,
                CreatedByClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == m.CreatedBy)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(modifications, "datawalletModifications.csv", progressReporter);
    }

    private async Task ExportSyncErrors(ProgressTask progressReporter)
    {
        var syncErrors = _adminApiDbContext
            .SyncErrors
            .Select(e => new SyncErrorExport
            {
                ErrorId = e.Id,
                SyncItemOwner = e.ExternalEvent.Owner,
                CreatedAt = e.SyncRun.FinalizedAt,
                ErrorCode = e.ErrorCode,
                SyncItemOwnerClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == e.ExternalEvent.Owner)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == e.ExternalEvent.Owner)!.ClientId)!
                            .DisplayName,
                SyncItemOwnerClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == e.ExternalEvent.Owner)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(syncErrors, "syncErrors.csv", progressReporter);
    }

    private async Task ExportTokens(ProgressTask progressReporter)
    {
        var modifications = _adminApiDbContext
            .Tokens
            .Select(t => new TokenExport
            {
                TokenId = t.Id,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                CipherSize = t.Content.Length,
                ExpiresAt = t.ExpiresAt,
                CreatedByClientName =
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(modifications, "tokens.csv", progressReporter);
    }

    private async Task StreamToCSV<T>(IAsyncEnumerable<T> objects, string filename, ProgressTask progressReporter) where T : notnull
    {
        await using var outputFileStream = new StreamWriter(Path.Join(_pathToExportDirectory, filename), append: false);

        var headerLine = string.Join(",", typeof(T).GetProperties().Select(p => p.Name));
        await outputFileStream.WriteLineAsync(headerLine);

        var total = await objects.CountAsync();

        var current = 0;
        await foreach (var obj in objects)
        {
            await outputFileStream.WriteLineAsync(obj.ToCsv());
            current++;
            progressReporter.Value = (double)current / total * 100;
        }

        progressReporter.Value = 100;
    }

    private void ZipExportDirectory()
    {
        if (File.Exists(_pathToZipFile))
            File.Delete(_pathToZipFile);

        ZipFile.CreateFromDirectory(_pathToExportDirectory, _pathToZipFile);
    }
}
