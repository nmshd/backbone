using Backbone.DevelopmentKit.Identity.ValueObjects;
using Serilog.Enrichers.Sensitive;

namespace Backbone.BuildingBlocks.API.Serilog;
public class IdentityAddressMaskingOperator : RegexMaskingOperator
{
    public IdentityAddressMaskingOperator(string regexString) : base(regexString) { }
    private static string IdentityAddressRegex => IdentityAddress.IdentityAddressValidatorRegex().ToString();
    public static IdentityAddressMaskingOperator Create() => new(IdentityAddressRegex[1..^1]);
}
