using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class BackboneIdMaskingOperator : RegexMaskingOperator
{
    public BackboneIdMaskingOperator(string regexString) : base(regexString) { }
    private static string IdRegexPattern(string prefix, int maxLength) => $@"{prefix}[a-zA-Z0-9]{{{maxLength}}}";
    public static BackboneIdMaskingOperator ForId(string prefix, int maxLength) => new(IdRegexPattern(prefix, maxLength));
}
