using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

[ApplyQuotasForMetrics("NumberOfFiles", "UsedFileStorageSpace")]
public class CreateFileCommand : IRequest<CreateFileResponse>
{
    public required byte[] FileContent { get; init; }

    public required byte[] OwnerSignature { get; init; }

    public required byte[] CipherHash { get; init; }

    public required DateTime ExpiresAt { get; init; }

    public required byte[] EncryptedProperties { get; init; }
}
