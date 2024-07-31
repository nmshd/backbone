using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class IdentityAddressMaskingOperator : RegexMaskingOperator
{
    public IdentityAddressMaskingOperator() : base(IdentityAddressRegex[1..^1]) { }
    private static string IdentityAddressRegex => IdentityAddress.IdentityAddressValidatorRegex().ToString();
}
