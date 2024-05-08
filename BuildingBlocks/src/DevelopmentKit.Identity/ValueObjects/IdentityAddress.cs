using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using SimpleBase;


namespace Backbone.DevelopmentKit.Identity.ValueObjects;

[Serializable]
[TypeConverter(typeof(IdentityAddressTypeConverter))]
public partial record IdentityAddress : StronglyTypedId
{
    public const int MAX_LENGTH = 80;

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
        
        var calculatedChecksum = CalculateChecksum(stringValue[..^2]);

        var checksumIsValid = givenChecksum.Value == calculatedChecksum;

        return checksumIsValid;

    }

    public static IdentityAddress Create(byte[] publicKey, string instanceUrl)
    {
        var hashedPublicKey = SHA256.HashData(SHA512.HashData(publicKey))[..10];

        var identitySpecificPart = Hex(hashedPublicKey);

        var mainPhrase = $"did:e:{instanceUrl}:dids:{identitySpecificPart}";
        var checksum = CalculateChecksum(mainPhrase);

        return new IdentityAddress((mainPhrase + checksum).ToLower());
    }

    private static string CalculateChecksum(string phrase) => Hex(SHA256.HashData(Encoding.ASCII.GetBytes(phrase)))[..2];

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

    [GeneratedRegex(@"^did\:e\:(?<instanceUrl>(?:[a-z0-9]+\.)+[a-z]{2,})\:dids\:(?<identitySpecificPart>[0-9abcdef]{20})(?<checksum>[0-9abcdef]{2})$", RegexOptions.IgnoreCase)]
    private static partial Regex IdentityAddressValidatorRegex();

    #endregion
}
