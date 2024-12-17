using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public interface IHasher
{
    byte[] HashUtf8(string input);
}

public static class Hasher
{
    private static readonly ThreadLocal<Func<IHasher>> GET_HASHER = new(() => () => new HasherImpl());

    public static void SetHasher(IHasher hasher)
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;

        if (callerType is { Namespace: not null } && !callerType.Namespace.Contains("Test"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }

        GET_HASHER.Value = () => hasher;
    }

    public static byte[] HashUtf8(string input)
    {
        return GET_HASHER.Value!().HashUtf8(input);
    }

    public static void Reset()
    {
        GET_HASHER.Value = () => new HasherImpl();
    }
}

internal class HasherImpl : IHasher
{
    private static readonly byte[] SALT = SHA256.HashData("enmeshed_identity_deletion_log"u8.ToArray());

    public byte[] HashUtf8(string input)
    {
        var hash = KeyDerivation.Pbkdf2(input, SALT, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return hash;
    }
}
