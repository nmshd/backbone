using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;

public class StronglyTypedIdMaskingOperator : RegexMaskingOperator
{
    public StronglyTypedIdMaskingOperator(string prefix, int maxLength = -1) : base(IsStringOfConstantLength(maxLength) ? RegexPattern(prefix, maxLength) : RegexPattern(prefix))
    {
    }

    private static string RegexPattern(string initials) => $@"{initials}[a-zA-Z0-9]*";
    private static string RegexPattern(string prefix, int maxLength) => $@"{prefix}[a-zA-Z0-9]{{{maxLength - prefix.Length}}}";

    private static bool IsStringOfConstantLength(int maxLength) => maxLength != -1;
}
