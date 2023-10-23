using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Files.Application.Files.Commands.CreateFile;

[ApplyQuotasForMetrics("NumberOfFiles", "UsedFileStorageSpace")]
public class CreateFileCommand : IRequest<CreateFileResponse>
{
    public byte[] FileContent { get; set; }

    public IdentityAddress Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }
}
