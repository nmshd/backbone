using Microsoft.AspNetCore.Http;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokens;

public class ListTokensQueryItem
{
    public required string Id { get; set; }

    internal static List<string> ReadIds(IQueryCollection query)
    {
        // Preserve the legacy MVC complex-array syntax and its contiguous indexing.
        var ids = new List<string>();
        for (var i = 0; query.TryGetValue($"tokens.{i}.id", out var id); i++)
            ids.Add(id.ToString());
        return ids;
    }
}
