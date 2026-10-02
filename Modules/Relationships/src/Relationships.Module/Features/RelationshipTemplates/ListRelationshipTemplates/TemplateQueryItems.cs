using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Relationships.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

internal static class TemplateQueryItems
{
    public static List<ListRelationshipTemplatesQueryItem> Read(IQueryCollection query)
    {
        var items = new List<ListRelationshipTemplatesQueryItem>();
        // Preserve the GenericArrayModelBinder's dotted keys and contiguous indices.
        for (var i = 0; ; i++)
        {
            var hasId = query.TryGetValue($"templates.{i}.id", out var id);
            var hasPassword = query.TryGetValue($"templates.{i}.password", out var password);
            if (!hasId && !hasPassword)
                break;
            if (!hasId)
                throw new BadHttpRequestException("A template id is required.");

            byte[]? decodedPassword = null;
            if (hasPassword)
            {
                if (!Base64QueryValue.TryParse(password.ToString(), out var value))
                    throw new BadHttpRequestException("The template password is not valid Base64.");
                decodedPassword = password.ToString().Length == 0 ? [] : value.Value;
            }
            items.Add(new ListRelationshipTemplatesQueryItem { Id = id.ToString(), Password = decodedPassword });
        }
        return items;
    }
}
