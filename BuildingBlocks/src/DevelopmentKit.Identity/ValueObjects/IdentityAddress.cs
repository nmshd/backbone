using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using SimpleBase;

namespace Backbone.DevelopmentKit.Identity.ValueObjects;

[Serializable]
[TypeConverter(typeof(IdentityAddressTypeConverter))]
public record IdentityAddress : StronglyTypedId
{
    public const int MAX_LENGTH = 36;

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

        var realm = stringValue[..3];

        var concatenation = Base58.Bitcoin.Decode(stringValue.AsSpan(3)).ToArray();
        var hashedPublicKey = concatenation[..20];
        var givenChecksum = concatenation[20..];

        var realmBytes = Encoding.UTF8.GetBytes(realm);
        var correctChecksum = CalculateChecksum(realmBytes, hashedPublicKey);

        var checksumIsValid = givenChecksum.SequenceEqual(correctChecksum);

        return lengthIsValid && checksumIsValid;
    }

    public static IdentityAddress Create(byte[] publicKey, string realm)
    {
        var hashedPublicKey = SHA256.Create().ComputeHash(SHA512.Create().ComputeHash(publicKey))[..20];
        var realmBytes = Encoding.UTF8.GetBytes(realm);
        var checksum = CalculateChecksum(realmBytes, hashedPublicKey);
        var concatenation = hashedPublicKey.Concat(checksum).ToArray();
        var address = realm + Base58.Bitcoin.Encode(concatenation);

        return new IdentityAddress(address);
    }

    private static byte[] CalculateChecksum(byte[] realmBytes, byte[] hashedPublicKey)
    {
        var checksumSource = realmBytes.Concat(hashedPublicKey).ToArray();
        var checksumHash = SHA256.Create().ComputeHash(SHA512.Create().ComputeHash(checksumSource));
        var checksum = checksumHash[..4];
        return checksum;
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
