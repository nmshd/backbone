﻿using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
    public byte[] HashUtf8(string input)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(input));
    }
}
