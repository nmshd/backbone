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

        var includeSensitiveData = new Option<bool?>("--sensitive")
        {
            Required = false,
            Description = "If this is set, sensitive data like IDs or identity addresses are exported as well."
        };

        Options.Add(includeSensitiveData);

        SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            var includeSensitiveDataValue = parseResult.GetValue(includeSensitiveData) ?? false;
            return ExportDatabase(includeSensitiveDataValue);
        });
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

    private async Task ExportDatabase(bool includeSensitiveData = false)
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
                await ExportDevices(includeSensitiveData, exportDevicesTask);

                exportDeletionAuditLogItemsTask.StartTask();
                await ExportDeletionAuditLogItems(exportDeletionAuditLogItemsTask);

                exportRelationshipTemplatesTask.StartTask();
                await ExportRelationshipTemplates(includeSensitiveData, exportRelationshipTemplatesTask);

                exportRelationshipsTask.StartTask();
                await ExportRelationships(includeSensitiveData, exportRelationshipsTask);

                exportFilesTask.StartTask();
                await ExportFiles(includeSensitiveData, exportFilesTask);

                exportMessagesTask.StartTask();
                await ExportMessages(includeSensitiveData, exportMessagesTask);

                exportDatawalletModificationsTask.StartTask();
                await ExportDatawalletModifications(includeSensitiveData, exportDatawalletModificationsTask);

                exportTokensTask.StartTask();
                await ExportTokens(includeSensitiveData, exportTokensTask);

                exportSyncErrorsTask.StartTask();
                await ExportSyncErrors(includeSensitiveData, exportSyncErrorsTask);
            });

        ZipExportDirectory();

        DeleteExportDirectory();
        Console.WriteLine(
            $"""
             Export completed. The zip file was saved at the following location: {_pathToZipFile}. If you ran this command from Kubernetes, you can copy it to your local system by running the following command:
             kubectl cp <namespace-name><pod-name>:{_pathToZipFile} <local-destination-path>
             """);
    }

    private async Task ExportDevices(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var devices = _adminApiDbContext
            .Devices
            .Select(d => new DeviceExport
            {
                DeviceId = includeSensitiveData ? d.Id : "",
                LastLoginAt = d.User.LastLoginAt,
                IdentityAddress = includeSensitiveData ? d.Identity.Address : "",
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

    private async Task ExportRelationshipTemplates(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var templates = _adminApiDbContext
            .RelationshipTemplates
            .Select(t => new RelationshipTemplateExport
            {
                TemplateId = includeSensitiveData ? t.Id : "",
                CreatedBy = includeSensitiveData ? t.CreatedBy : "",
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

    private async Task ExportRelationships(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var relationships = _adminApiDbContext
            .Relationships
            .Select(r => new RelationshipExport
            {
                RelationshipId = includeSensitiveData ? r.Id : "",
                TemplateId = includeSensitiveData ? r.RelationshipTemplateId == null ? null : r.RelationshipTemplateId.ToString() : "",
                From = includeSensitiveData ? r.From : "",
                To = includeSensitiveData ? r.To : "",
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

    private async Task ExportFiles(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var files = _adminApiDbContext
            .Files
            .Select(f => new FileExport
            {
                FileId = includeSensitiveData ? f.Id : "",
                CreatedBy = includeSensitiveData ? f.CreatedBy : "",
                CreatedAt = f.CreatedAt,
                LastOwnershipClaimAt = f.LastOwnershipClaimAt,
                Owner = includeSensitiveData ? f.Owner : "",
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

    private async Task ExportMessages(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var messages = _adminApiDbContext
            .Messages
            .Select(m => new MessageExport
            {
                MessageId = includeSensitiveData ? m.Id : "",
                CreatedBy = includeSensitiveData ? m.CreatedBy : "",
                RelationshipId = includeSensitiveData ? m.Recipients.First().RelationshipId : "",
                Recipient = includeSensitiveData ? m.Recipients.First().Address : "",
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

    private async Task ExportDatawalletModifications(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var modifications = _adminApiDbContext
            .DatawalletModifications
            .Select(m => new DatawalletModificationExport
            {
                DatawalletModificationId = includeSensitiveData ? m.Id : "",
                CreatedAt = m.CreatedAt,
                CreatedBy = includeSensitiveData ? m.CreatedBy : "",
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

    private async Task ExportSyncErrors(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var syncErrors = _adminApiDbContext
            .SyncErrors
            .Select(e => new SyncErrorExport
            {
                ErrorId = e.Id,
                SyncItemOwner = includeSensitiveData ? e.ExternalEvent.Owner : "",
                CreatedAt = e.CreatedAt,
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

    private async Task ExportTokens(bool includeSensitiveData, ProgressTask progressReporter)
    {
        var tokens = _adminApiDbContext
            .Tokens
            .Select(t => new TokenExport
            {
                TokenId = includeSensitiveData ? t.Id : "",
                CreatedAt = t.CreatedAt,
                CreatedBy = includeSensitiveData ? t.CreatedBy : "",
                CipherSize = t.Content == null ? 0 : t.Content.Length,
                ExpiresAt = t.ExpiresAt,
                CreatedByClientName =
                    t.CreatedBy == null ||
                    _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId) == null
                        ? null
                        : _adminApiDbContext.OpenIddictApplications.FirstOrDefault(a => a.ClientId == _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId)!
                            .DisplayName,
                CreatedByClientId = t.CreatedBy == null ? null : _adminApiDbContext.Identities.FirstOrDefault(i => i.Address == t.CreatedBy)!.ClientId
            })
            .ToAsyncEnumerable();

        await StreamToCSV(tokens, "tokens.csv", progressReporter);
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
