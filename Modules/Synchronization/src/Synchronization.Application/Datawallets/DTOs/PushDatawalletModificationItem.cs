using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Synchronization.Application.Datawallets.DTOs;

public class PushDatawalletModificationItem
{
    public string ObjectIdentifier { get; set; }
    public string PayloadCategory { get; set; }
    public string Collection { get; set; }
    public DatawalletModificationDTO.DatawalletModificationType Type { get; set; }
    public byte[] EncryptedPayload { get; set; }
    public ushort DatawalletVersion { get; set; }
}

public class PushDatawalletModificationItemValidator : AbstractValidator<PushDatawalletModificationItem>
{
    public PushDatawalletModificationItemValidator()
    {
        RuleFor(i => i.Type).DetailedNotNull();
        RuleFor(i => i.Collection).DetailedNotNull();
        RuleFor(i => i.ObjectIdentifier).DetailedNotNull();
    }
}
