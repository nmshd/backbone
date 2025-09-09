namespace Backbone.Job.IdentityDeletion;

public static class StringArrayExtensions
{
    public static string ExtractString(this string[] parts, string prefix) => parts.Single(x => x.StartsWith(prefix))[prefix.Length..];
}
