using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;

public class BackboneIdMaskingOperator : RegexMaskingOperator
{
    public BackboneIdMaskingOperator(string initials, int length = -1) : base(IsStringOfConstantLength(length) ? RegexPattern(initials, length) : RegexPattern(initials)) { }

    private static bool IsStringOfConstantLength(int length) => length != -1;
    private static string RegexPattern(string initials) => $@"{initials}[a-zA-Z0-9]*";
    private static string RegexPattern(string initials, int length) => $@"{initials}[a-zA-Z0-9]{{{length}}}";
}
