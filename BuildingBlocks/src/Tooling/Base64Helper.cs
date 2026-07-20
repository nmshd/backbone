using System.Buffers.Text;

namespace Backbone.Tooling;

public static class Base64Helper
{
    public static byte[] Decode(string base64)
    {
        try
        {
            return Base64Url.DecodeFromChars(base64);
        }
        catch (FormatException)
        {
            return Convert.FromBase64String(base64);
        }
    }

    public static string EncodeUrlSafeWithoutPadding(byte[] value)
    {
        return new string(Base64Url.EncodeToChars(value));
    }
}
