using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

[ApplyQuotasForMetrics("NumberOfFiles", "UsedFileStorageSpace (MB)")]
public class CreateFileCommand : IRequest<CreateFileResponse>
{
    public byte[] FileContent { get; set; }

    public IdentityAddress Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }
}
