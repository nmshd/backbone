using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Synchronization.Domain.Entities;
using static Synchronization.Domain.Entities.Datawallet;

namespace Synchronization.Application.Tests;

public class DatawalletModificationBuilder
{
    private string _collection;
    private DeviceId _createdByDevice;
    private Datawallet _datawallet;
    private byte[] _encryptedPayload;
    private long _index;
    private string _objectIdentifier;
    private string _payloadCategory;
    private DatawalletModificationType _type;
    private DatawalletVersion _version = new(1);

    public DatawalletModificationBuilder()
    {
        _index = 0;
        _type = DatawalletModificationType.Create;
        _collection = "ACollection";
        _objectIdentifier = "AnObjectIdentifier";
        _createdByDevice = TestDataGenerator.CreateRandomDeviceId();
        _payloadCategory = "APayloadCategory";
        _encryptedPayload = TestDataGenerator.CreateRandomBytes();
    }

    public DatawalletModificationBuilder WithIndex(long value)
    {
        _index = value;
        return this;
    }

    public DatawalletModificationBuilder WithType(DatawalletModificationType value)
    {
        _type = value;
        return this;
    }

    public DatawalletModificationBuilder WithCollection(string value)
    {
        _collection = value;
        return this;
    }

    public DatawalletModificationBuilder WithObjectIdentifier(string value)
    {
        _objectIdentifier = value;
        return this;
    }

    public DatawalletModificationBuilder WithCreatedByDevice(DeviceId value)
    {
        _createdByDevice = value;
        return this;
    }

    public DatawalletModificationBuilder WithPayloadCategory(string value)
    {
        _payloadCategory = value;
        return this;
    }

    public DatawalletModificationBuilder WithDatawallet(Datawallet datawallet)
    {
        _datawallet = datawallet;
        return this;
    }

    public DatawalletModificationBuilder WithEncryptedPayload(byte[] value)
    {
        _encryptedPayload = value;
        return this;
    }

    public DatawalletModificationBuilder WithVersion(DatawalletVersion value)
    {
        _version = value;
        return this;
    }

    public DatawalletModification Build()
    {
        if (_datawallet == null)
            throw new Exception("datawallet cannot be null");
        var modification = new DatawalletModification(_datawallet, _version, _index, _type, _collection, _objectIdentifier, _payloadCategory, _encryptedPayload, _createdByDevice);

        return modification;
    }
}
