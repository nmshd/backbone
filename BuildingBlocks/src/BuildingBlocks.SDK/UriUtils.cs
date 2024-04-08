namespace Backbone.BuildingBlocks.SDK;

public class UriUtils
{
    public static Uri CreateUriFromParts(params string[] parts)
        => new Uri(parts.Aggregate((a, b) => $"{a.TrimEnd('/')}/{b.TrimStart('/')}"));
}
