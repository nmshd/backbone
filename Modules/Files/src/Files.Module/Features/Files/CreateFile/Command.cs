using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.CreateFile;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateFileCommand")]
[ApplyQuotasForMetrics("NumberOfFiles", "UsedFileStorageSpace")]
public class Command : IRequest<Response>
{
    public required byte[] FileContent { get; init; }

    public required byte[] OwnerSignature { get; init; }

    public required byte[] CipherHash { get; init; }

    public required DateTime ExpiresAt { get; init; }

    public required byte[] EncryptedProperties { get; init; }
}
