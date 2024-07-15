using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class RecipientInformationDTO : IMapTo<RecipientInformation>
{
    public RecipientInformationDTO(RecipientInformation recipientInformation)
    {
        Address = recipientInformation.Address;
        EncryptedKey = recipientInformation.EncryptedKey;
        ReceivedAt = recipientInformation.ReceivedAt;
        ReceivedByDevice = recipientInformation.ReceivedByDevice;
    }

    public IdentityAddress Address { get; set; }
    public byte[] EncryptedKey { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DeviceId? ReceivedByDevice { get; set; }
}
