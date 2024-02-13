using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class PushDatawalletModificationItem
{
    public required string ObjectIdentifier { get; set; }
    public required string PayloadCategory { get; set; }
    public required string Collection { get; set; }
    public DatawalletModificationDTO.DatawalletModificationType Type { get; set; }
    public required byte[] EncryptedPayload { get; set; }
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
