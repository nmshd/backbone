using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Backbone.BuildingBlocks.Domain;
using SimpleBase;
using static System.Security.Cryptography.SHA256;

namespace Backbone.DevelopmentKit.Identity.ValueObjects;

[Serializable]
[TypeConverter(typeof(IdentityAddressTypeConverter))]
public class IdentityAddress : IFormattable, IEquatable<IdentityAddress>, IComparable<IdentityAddress>
{
    public const int MAX_LENGTH = 100;

    private IdentityAddress(string stringValue)
    {
        StringValue = stringValue;
    }

    public string StringValue { get; }

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

        const string pattern = @"^did\:web\:(.+?)\:dids\:(.+)$";
        var matches = Regex.Matches(stringValue, pattern, RegexOptions.IgnoreCase);
        if (matches.Count != 2)
        {
            return false;
        }
        try
        {
            var concatenation = Base58.Bitcoin.Decode(matches.First().Groups[2].Value).ToArray();

            var hashedPublicKey = concatenation[..20];
            var givenChecksum = concatenation[20..];

            var correctChecksum = CalculateChecksum(hashedPublicKey);

            var checksumIsValid = givenChecksum.SequenceEqual(correctChecksum);

            return lengthIsValid && checksumIsValid;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static IdentityAddress Create(byte[] publicKey, string instanceUrl)
    {
        var hashedPublicKey = HashData(HashData(publicKey))[..20];
        var checksum = CalculateChecksum(hashedPublicKey);
        var concatenation = hashedPublicKey.Concat(checksum).ToArray();
        var address = Base58.Bitcoin.Encode(concatenation);

        return new IdentityAddress($"did:web:{instanceUrl}:dids:{address}");
    }

    private static byte[] CalculateChecksum(byte[] hashedPublicKey) => HashData(HashData(hashedPublicKey))[..4];

    public override string ToString()
    {
        return StringValue;
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
        return identityAddress.StringValue;
    }

    public static implicit operator IdentityAddress(string stringValue)
    {
        return ParseUnsafe(stringValue);
    }

    #endregion

    #region Equality members

    public bool Equals(IdentityAddress? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StringValue == other.StringValue;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IdentityAddress)obj);
    }

    public override int GetHashCode()
    {
        return StringValue.GetHashCode();
    }

    public int CompareTo(IdentityAddress? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(StringValue, other.StringValue, StringComparison.Ordinal);
    }

    public static bool operator ==(IdentityAddress? left, IdentityAddress? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(IdentityAddress? left, IdentityAddress? right)
    {
        return !Equals(left, right);
    }

    #endregion
}
