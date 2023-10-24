namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public interface IHasher
{
    byte[] HashUtf8(string input);
}
