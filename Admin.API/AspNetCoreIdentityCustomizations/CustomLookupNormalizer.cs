using Microsoft.AspNetCore.Identity;

namespace Admin.API.AspNetCoreIdentityCustomizations;

public class CustomLookupNormalizer : ILookupNormalizer
{
    public string? NormalizeName(string? name)
    {
        return name?.Trim().Normalize().ToUpperInvariant();
    }

    public string? NormalizeEmail(string? email)
    {
        return email?.Trim().Normalize().ToUpperInvariant();
    }
}