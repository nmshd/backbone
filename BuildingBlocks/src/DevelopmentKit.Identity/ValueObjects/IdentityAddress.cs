using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.DevelopmentKit.Identity.ValueObjects;

[Serializable]
[TypeConverter(typeof(IdentityAddressTypeConverter))]
public partial record IdentityAddress : StronglyTypedId
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    public const int MAX_LENGTH = 80;
    private const int CHECKSUM_LENGTH = 2;
    private const string CHECKSUM_LENGTH_S = "2";

    private IdentityAddress(string stringValue) : base(stringValue)
    {
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ToString();
    }

    public static IdentityAddress Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(IdentityAddress)}.");

        return ParseUnsafe(stringValue);
    }

    /// <summary>
    ///     Call this method only if you know that the address is valid and you want to skip validation.
    /// </summary>
    public static IdentityAddress ParseUnsafe(string stringValue)
    {
        return new IdentityAddress(stringValue);
    }

    public static bool IsValid(string? stringValue)
    {
        if (stringValue == null) return false;

        if (stringValue.Length > MAX_LENGTH)
            return false;

        var matches = IdentityAddressValidatorRegex().Matches(stringValue);

        if (matches.Count == 0) return false;

        var matchGroups = matches.First().Groups;

        if (!matchGroups.TryGetValue("checksum", out var givenChecksum))
            return false;

        if (!matchGroups.TryGetValue("addressWithoutChecksum", out var addressWithoutChecksum))
            return false;

        var expectedChecksum = CalculateChecksum(addressWithoutChecksum.Value);

        var checksumIsValid = givenChecksum.Value == expectedChecksum;

        return checksumIsValid;
    }

    public static IdentityAddress Create(byte[] publicKey, string didDomainName)
    {
        var hashedPublicKey = SHA256.HashData(SHA512.HashData(publicKey))[..10];

        var identitySpecificPart = Hex(hashedPublicKey);

        var mainPhrase = $"did:e:{didDomainName}:dids:{identitySpecificPart}";
        var checksum = CalculateChecksum(mainPhrase);

        return new IdentityAddress((mainPhrase + checksum).ToLower());
    }

    public static IdentityAddress GetAnonymized(string didDomainName)
    {
        return Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), didDomainName);
    }

    private static string CalculateChecksum(string phrase) => Hex(SHA256.HashData(Encoding.ASCII.GetBytes(phrase)))[..CHECKSUM_LENGTH];

    private static string Hex(byte[] bytes)
    {
        return Convert.ToHexString(bytes).ToLower();
    }

    public override string ToString()
    {
        return Value;
    }

    #region Converters

    public class IdentityAddressTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue)
                ? Parse(stringValue)
                : base.ConvertFrom(context, culture, value)!;
        }
    }

    #endregion

    #region Operators

    public static implicit operator string(IdentityAddress identityAddress)
    {
        return identityAddress.Value;
    }

    public static implicit operator IdentityAddress(string stringValue)
    {
        return ParseUnsafe(stringValue);
    }

    [GeneratedRegex($@"^(?<addressWithoutChecksum>did:e:(?<didDomainName>(?:[a-z0-9-]+\.)*[a-z]{{2,}}):dids:(?<identitySpecificPart>[0-9a-f]{{20}}))(?<checksum>[0-9a-f]{{{CHECKSUM_LENGTH_S}}})$")]
    public static partial Regex IdentityAddressValidatorRegex();

    #endregion
}
