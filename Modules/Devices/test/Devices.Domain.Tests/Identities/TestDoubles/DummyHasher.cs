using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;

public class DummyHasher : IHasher
{
    private readonly byte[] _bytes;

    public DummyHasher()
    {
        _bytes = [1, 2, 3];
    }

    public DummyHasher(byte[] bytes)
    {
        _bytes = bytes;
    }

    public byte[] HashUtf8(string input)
    {
        return _bytes;
    }
}
