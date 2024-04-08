namespace Backbone.BuildingBlocks.SDK;

public class UriUtils
{
    public static Uri CreateUriFromParts(params string[] parts)
    {
        var path = parts.Aggregate((a, b) => $"{a.TrimEnd('/')}/{b.TrimStart('/')}");

        return new Uri(path);
    }
}
