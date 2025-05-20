using System.IO.Compression;
using System.Net.Mime;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class DatabaseExportController : ApiControllerBase
{
    private readonly DevicesDbContext _devicesDbContext;
    private readonly RelationshipsDbContext _relationshipsDbContext;
    private readonly FilesDbContext _filesDbContext;
    private readonly MessagesDbContext _messagesDbContext;
    private readonly SynchronizationDbContext _synchronizationDbContext;
    private readonly string _pathToExportDirectory = Path.Combine(Path.GetTempPath(), "Backbone", "DatabaseExport");
    private readonly string _pathToZipFile = Path.Combine(Path.GetTempPath(), "Backbone", "DatabaseExport.zip");
    private Dictionary<string, string?> _addressToClientDisplayName = null!;
    private readonly TokensDbContext _tokensDbContext;

    public DatabaseExportController(IMediator mediator, DevicesDbContext devicesDbContext, RelationshipsDbContext relationshipsDbContext, FilesDbContext filesDbContext,
        MessagesDbContext messagesDbContext, SynchronizationDbContext synchronizationDbContext, TokensDbContext tokensDbContext) : base(mediator)
    {
        _devicesDbContext = devicesDbContext;
        _relationshipsDbContext = relationshipsDbContext;
        _filesDbContext = filesDbContext;
        _messagesDbContext = messagesDbContext;
        _synchronizationDbContext = synchronizationDbContext;
        _tokensDbContext = tokensDbContext;
        CreateExportDirectoryIfNotExists();
    }

    private void CreateExportDirectoryIfNotExists()
    {
        if (!Directory.Exists(_pathToExportDirectory))
        {
            Directory.CreateDirectory(_pathToExportDirectory);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportDatabase()
    {
        _addressToClientDisplayName = await _devicesDbContext.Identities.ToDictionaryAsync(i => i.Address.ToString(),
            i => _devicesDbContext.Set<CustomOpenIddictEntityFrameworkCoreApplication>().First(c => c.ClientId == i.ClientId).DisplayName);

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

        var fileStream = new FileStream(_pathToZipFile, FileMode.Open, FileAccess.Read);
        return File(fileStream, MediaTypeNames.Application.Octet);
    }

    [HttpGet("{what}")]
    public async Task<IActionResult> ExportOnlyDevices(string what)
    {
        _addressToClientDisplayName = await _devicesDbContext.Identities.ToDictionaryAsync(i => i.Address.ToString(),
            i => _devicesDbContext.Set<CustomOpenIddictEntityFrameworkCoreApplication>().First(c => c.ClientId == i.ClientId).DisplayName);

        await (Task)GetType().GetMethod($"Export{what}")?.Invoke(this, [])!;

        var fileStream = new FileStream(Path.Join(_pathToExportDirectory, $"{what}.csv"), FileMode.Open, FileAccess.Read);
        var result = new FileStreamResult(fileStream, "text/csv");
        return result;
    }

    public async Task ExportDevices()
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

    public async Task ExportDeletionAuditLogItems()
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

    public async Task ExportRelationshipTemplates()
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

    public async Task ExportRelationships()
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

    public async Task ExportFiles()
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

    public async Task ExportMessages()
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

    public async Task ExportDatawalletModifications()
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

    public async Task ExportSyncErrors()
    {
        var syncErrors = _synchronizationDbContext
            .SyncErrors
            .Select(e => new SyncErrorExport
            {
                SyncItemOwner = e.ExternalEvent.Owner,
                CreatedAt = e.SyncRun.FinalizedAt!.Value,
                ErrorCode = e.ErrorCode
            })
            .ToAsyncEnumerable();

        await StreamToCSV(syncErrors, "syncErrors.csv", m => { m.SyncItemOwnerClientName = GetClientName(m.SyncItemOwner); });
    }

    public async Task ExportTokens()
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
        if (System.IO.File.Exists(_pathToZipFile))
            System.IO.File.Delete(_pathToZipFile);

        ZipFile.CreateFromDirectory(_pathToExportDirectory, _pathToZipFile);
    }
}

public class DeviceExport
{
    public required string DeviceId { get; set; } = null!;
    public required DateTime? LastLoginAt { get; set; }
    public required string IdentityAddress { get; set; } = null!;
    public required DateTime CreatedAt { get; set; }
    public required string Tier { get; set; } = null!;
    public required IdentityStatus IdentityStatus { get; set; }
    public required DateTime? IdentityDeletionGracePeriodEndsAt { get; set; }

    public string? ClientName { get; set; }
    public required PushNotificationPlatform? Platform { get; set; } = null!;
}

public class DeletionAuditLogItemExport
{
    public required DeletionProcessStatus? OldStatus { get; set; }
    public required DeletionProcessStatus? NewStatus { get; set; }
    public required byte[] IdentityAddressHash { get; set; }
    public required MessageKey MessageKey { get; set; }
    public required DateTime CreatedAt { get; set; }
}

public class RelationshipTemplateExport
{
    public required string TemplateId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? AllocatedAt { get; set; }
}

public class RelationshipExport
{
    public required string RelationshipId { get; set; }
    public required string? TemplateId { get; set; }
    public required string From { get; set; }
    public string? FromClientName { get; set; }
    public required string To { get; set; }
    public string? ToClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required RelationshipStatus Status { get; set; }
    public required bool FromHasDecomposed { get; set; }
    public required bool ToHasDecomposed { get; set; }
    public required DateTime? TemplateCreatedAt { get; set; }
}

public class FileExport
{
    public required string FileId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Owner { get; set; }
    public string? OwnerClientName { get; set; }
    public required long CipherSize { get; set; }
    public required DateTime ExpiresAt { get; set; }
}

public class MessageExport
{
    public required string MessageId { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required string RelationshipId { get; set; }
    public required string Recipient { get; set; }
    public string? RecipientClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ReceivedAt { get; set; }
    public required long CipherSize { get; set; }
}

public class DatawalletModificationExport
{
    public required string DatawalletModificationId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required string ObjectIdentifier { get; set; }
    public required string Collection { get; set; }
    public required DatawalletModificationType Type { get; set; }
    public required string? PayloadCategory { get; set; }
    public required long? PayloadSize { get; set; }
}

public class SyncErrorExport
{
    public required string SyncItemOwner { get; set; }
    public string? SyncItemOwnerClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string ErrorCode { get; set; }
}

public class TokenExport
{
    public required string TokenId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public string? CreatedByClientName { get; set; }
    public required long CipherSize { get; set; }
    public required DateTime ExpiresAt { get; set; }
}

public static class Extensions
{
    public static string ToCsv(this object obj)
    {
        static string? ObjToString(object? obj)
        {
            if (obj?.GetType() == typeof(byte[]))
                return Convert.ToBase64String((byte[])obj);

            return obj?.ToString();
        }

        var properties = obj.GetType().GetProperties();
        var values = properties.Select(p => ObjToString(p.GetValue(obj)) ?? string.Empty);
        return string.Join(",", values);
    }
}
