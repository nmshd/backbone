﻿using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Enmeshed.UnitTestTools.Data;

public static class TestDataGenerator
{
    public static string GenerateString(int resultLength, char[]? chars = null)
    {
        chars ??= new char[] { 'A', 'B', 'C' };

        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static IdentityAddress CreateRandomIdentityAddress()
    {
        return IdentityAddress.Create(CreateRandomBytes(), "id1");
    }

    public static byte[] CreateRandomBytes()
    {
        var random = new Random();
        var bytes = new byte[10];
        random.NextBytes(bytes);
        return bytes;
    }
}

