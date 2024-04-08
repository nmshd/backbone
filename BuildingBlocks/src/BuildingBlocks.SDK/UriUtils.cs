namespace Backbone.BuildingBlocks.SDK;

public class UriUtils
{
    public static Uri CreateUriFromParts(params string[] parts) => new(parts.Aggregate((a, b) => $"{a.TrimEnd('/')}/{b.TrimStart('/')}"));
}
