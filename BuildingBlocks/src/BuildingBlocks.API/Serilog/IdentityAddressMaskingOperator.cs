using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class IdentityAddressMaskingOperator : RegexMaskingOperator
{
    public IdentityAddressMaskingOperator() : base(TrimRegexForMidStringUse()) { }

    private static string IdentityAddressRegex => IdentityAddress.IdentityAddressValidatorRegex().ToString();

    private static string TrimRegexForMidStringUse()
    {
        return IdentityAddressRegex[1..^1];
    }
}
