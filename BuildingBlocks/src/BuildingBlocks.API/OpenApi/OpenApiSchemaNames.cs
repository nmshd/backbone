namespace Backbone.BuildingBlocks.API.OpenApi;

public class OpenApiSchemaNames
{
    public Dictionary<Type, string> Overrides { get; } = [];

    public string GetSchemaId(Type type)
    {
        if (type.IsGenericType)
        {
            var typeName = type.Name
                .Replace("HttpResponseEnvelopeResult", "ResponseWrapper")
                .Replace("PagedHttpResponseEnvelopeResult", "PagedResponseWrapper");
            return $"{typeName[..typeName.IndexOf('`')]}_{string.Join("_", type.GetGenericArguments().Select(GetSchemaId))}";
        }

        if (Overrides.TryGetValue(type, out var overrideName))
            return Normalize(overrideName);

        var name = type.Name;
        if (type.Namespace is { } ns && ns.StartsWith("Backbone.Modules.", StringComparison.Ordinal) &&
            ns.Contains(".Module.Features.", StringComparison.Ordinal) &&
            name is "Command" or "Query" or "Response" or "Request" or "RequestBody" or "Validator")
        {
            var sliceName = ns[(ns.LastIndexOf('.') + 1)..];
            name = sliceName + (name == "RequestBody" ? "Request" : name);
        }

        return Normalize(name);
    }

    private static string Normalize(string name) => name
        .Replace("DTO", string.Empty)
        .Replace("Command", "Request")
        .Replace("Query", "Request");
}
