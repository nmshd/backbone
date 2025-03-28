using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;

public class StronglyTypedIdMaskingOperator : RegexMaskingOperator
{
    public StronglyTypedIdMaskingOperator(string prefix, int maxLength) : base(GetRegex(prefix, maxLength))
    {
    }

    private static string GetRegex(string prefix, int maxLength)
    {
        const string idCharacters = "a-zA-Z0-9";
        var lengthWithoutPrefix = maxLength - prefix.Length;

        var id = $"{prefix}[{idCharacters}]{{{lengthWithoutPrefix}}}";

        return $"(?<![{idCharacters}]){id}(?![{idCharacters}])";
    }
}
