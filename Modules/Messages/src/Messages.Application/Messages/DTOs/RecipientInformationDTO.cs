using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Messages.Domain.Entities;

namespace Backbone.Messages.Application.Messages.DTOs;

public class RecipientInformationDTO : IMapTo<RecipientInformation>
{
    public IdentityAddress Address { get; set; }
    public byte[] EncryptedKey { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DeviceId ReceivedByDevice { get; set; }
}
