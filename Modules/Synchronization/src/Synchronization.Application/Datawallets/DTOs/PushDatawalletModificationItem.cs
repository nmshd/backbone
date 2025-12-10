using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class PushDatawalletModificationItem
{
    public required DatawalletModificationDTO.DatawalletModificationType Type { get; set; }
    public required string ObjectIdentifier { get; set; }
    public required string Collection { get; set; }
    public string? PayloadCategory { get; set; }
    public byte[]? EncryptedPayload { get; set; }
    public required ushort DatawalletVersion { get; set; }
}

public class PushDatawalletModificationItemValidator : AbstractValidator<PushDatawalletModificationItem>
{
    public PushDatawalletModificationItemValidator()
    {
        RuleFor(i => i.Type).DetailedNotNull();
        RuleFor(i => i.PayloadCategory).DetailedMaximumLength(50);
        RuleFor(i => i.Collection).DetailedNotNull().DetailedMaximumLength(50);
        RuleFor(i => i.ObjectIdentifier).DetailedNotNull().DetailedMaximumLength(100);
    }
}
