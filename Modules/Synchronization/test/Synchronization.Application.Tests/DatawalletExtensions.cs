using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Tests;

public static class DatawalletExtensions
{
    public static DatawalletModification AddModification(this Datawallet datawallet, AddModificationParameters parameters)
    {
        return datawallet.AddModification(parameters.Type, parameters.DatawalletVersion, parameters.Collection, parameters.ObjectIdentifier, parameters.PayloadCategory, parameters.EncryptedPayload, parameters.CreatedByDevice, "");
    }

    public class AddModificationParameters
    {
        public string ObjectIdentifier { get; init; } = "anIdentifier";
        public string PayloadCategory { get; init; } = "aPayloadCategory";
        public DeviceId CreatedByDevice { get; init; } = TestDataGenerator.CreateRandomDeviceId();
        public string Collection { get; init; } = "aCollection";
        public DatawalletModificationType Type { get; init; } = DatawalletModificationType.Create;
        public byte[] EncryptedPayload { get; init; } = TestDataGenerator.CreateRandomBytes();
        public Datawallet.DatawalletVersion DatawalletVersion { get; init; } = new(1);
    }
}
