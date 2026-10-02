using Backbone.Tooling;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public readonly record struct Base64QueryValue(byte[]? Value)
{
    public static bool TryParse(string? value, out Base64QueryValue result)
    {
        result = default;
        if (string.IsNullOrEmpty(value))
            return true;

        try
        {
            result = new Base64QueryValue(Base64Helper.Decode(value));
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
