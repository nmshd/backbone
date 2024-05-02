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
public record IdentityAddress : StronglyTypedId
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

        var lengthIsValid = stringValue.Length <= MAX_LENGTH;

        const string pattern = @"^did\:e\:(.+?)\:dids\:(.+)(.{2})$";
        try
        {
            var matches = Regex.Matches(stringValue, pattern, RegexOptions.IgnoreCase).First().Groups;

            var givenChecksum = matches[3].Value;
            var calculatedChecksum = CalculateChecksum(stringValue[..^2]);

            var checksumIsValid = givenChecksum == calculatedChecksum;

            return lengthIsValid && checksumIsValid;
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentException or FormatException or InvalidOperationException)
        {
            return false;
        }
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

    private static string Hex(byte[] hashedPublicKey)
    {
        return Convert.ToHexString(hashedPublicKey).ToLower();
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

    #endregion
}
