using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

[ApplyQuotasForMetrics("NumberOfFiles", "UsedFileStorageSpace")]
public class CreateFileCommand : IRequest<CreateFileResponse>
{
    public required byte[] FileContent { get; set; }

    public required byte[] OwnerSignature { get; set; }

    public required byte[] CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public required byte[] EncryptedProperties { get; set; }
}
