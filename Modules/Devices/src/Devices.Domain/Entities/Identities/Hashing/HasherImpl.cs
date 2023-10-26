namespace Backbone.Modules.Devices.Domain.Entities.Identities.Hashing;

internal class HasherImpl : IHasher
{
    public byte[] HashUtf8(string input)
    {
        return Array.Empty<byte>(); // TODO: implement real hashing
    }
}
