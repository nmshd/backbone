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
        const string forbiddenLeadingCharacters = $"(?<![{idCharacters}])";
        const string forbiddenTrailingCharacters = $"(?![{idCharacters}])";

        var lengthWithoutPrefix = maxLength - prefix.Length;

        var potentialId = $"{prefix}[{idCharacters}]{{{lengthWithoutPrefix}}}";

        return $"{forbiddenLeadingCharacters}{potentialId}{forbiddenTrailingCharacters}";
    }
}
