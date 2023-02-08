﻿using System.Security.Cryptography;
using System.Text;

namespace Backbone.Modules.Devices.AdminCli;

public static class Password
{
    public static string Generate(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        var passwordBuilder = new StringBuilder();

        while (0 < length--)
        {
            passwordBuilder.Append(validChars[RandomNumberGenerator.GetInt32(0, validChars.Length - 1)]);
        }

        return passwordBuilder.ToString();
    }
}
