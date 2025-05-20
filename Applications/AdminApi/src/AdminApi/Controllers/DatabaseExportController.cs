using System.IO.Compression;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
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
    private readonly string _pathToExportDirectory = Path.Combine(Path.GetTempPath(), "Backbone", "DatabaseExport");
    private readonly string _pathToZipFile = Path.Combine(Path.GetTempPath(), "Backbone", "DatabaseExport.zip");
    private Dictionary<string, string?> _addressToClientDisplayName = null!;

    public DatabaseExportController(IMediator mediator, DevicesDbContext devicesDbContext, RelationshipsDbContext relationshipsDbContext, FilesDbContext filesDbContext) : base(mediator)
    {
        _devicesDbContext = devicesDbContext;
        _relationshipsDbContext = relationshipsDbContext;
        _filesDbContext = filesDbContext;
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
        // await ExportMessages();
        // await ExportDatawalletModifications();
        // await ExportTokens();
        // await ExportSyncErrors();
        // await ExportDeletionAuditLogItems();

        ZipExportDirectory();

        var fileStream = new FileStream(_pathToZipFile, FileMode.Open, FileAccess.Read);
        var result = new FileStreamResult(fileStream, "application/zip");
        return result;
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

    public async Task ExportDeletionAuditLogItems()
    {
        throw new NotImplementedException();
    }

    public async Task ExportSyncErrors()
    {
        throw new NotImplementedException();
    }

    public async Task ExportTokens()
    {
        throw new NotImplementedException();
    }

    public async Task ExportDatawalletModifications()
    {
        throw new NotImplementedException();
    }

    public async Task ExportMessages()
    {
        throw new NotImplementedException();
    }

    private string? GetClientName(string address)
    {
        return _addressToClientDisplayName.TryGetValue(address, out var clientName) ? clientName : null;
    }

    private async Task StreamToCSV<T>(IAsyncEnumerable<T> objects, string filename, Action<T> enricher) where T : notnull
    {
        await using var outputFileStream = new StreamWriter(Path.Join(_pathToExportDirectory, filename), append: false);

        var header = string.Join(",", typeof(T).GetProperties().Select(p => p.Name));
        await outputFileStream.WriteLineAsync(header);

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

public static class Extensions
{
    public static string ToCsv(this object obj)
    {
        var properties = obj.GetType().GetProperties();
        var values = properties.Select(p => p.GetValue(obj)?.ToString() ?? string.Empty);
        return string.Join(",", values);
    }
}
