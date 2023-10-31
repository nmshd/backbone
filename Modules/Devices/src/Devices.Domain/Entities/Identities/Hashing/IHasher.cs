namespace Backbone.Modules.Devices.Domain.Entities.Identities.Hashing;

public interface IHasher
{
    byte[] HashUtf8(string input);
}
