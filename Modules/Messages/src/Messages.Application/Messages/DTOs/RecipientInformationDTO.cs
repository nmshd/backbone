using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class RecipientInformationDTO : IMapTo<RecipientInformation>
{
    public IdentityAddress Address { get; set; }
    public byte[] EncryptedKey { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DeviceId ReceivedByDevice { get; set; }
}
