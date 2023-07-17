using Enmeshed.Tooling.Extensions;

namespace Backbone.Modules.Devices.AdminCli;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return value == null || value.IsEmpty();
    }
}
