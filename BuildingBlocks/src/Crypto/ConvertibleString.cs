using System.Text;

namespace Backbone.Crypto;

public class ConvertibleString
{
    public byte[] BytesRepresentation { get; }
    public string Base64Representation { get; }
    public string Utf8Representation { get; }
    public int SizeInBits => BytesRepresentation.Length * 8;
    public int SizeInBytes => BytesRepresentation.Length;

    public static ConvertibleString FromBase64(string base64String)
    {
        return new ConvertibleString(base64String);
    }

    public static ConvertibleString FromUtf8(string utf8String)
    {
        return new ConvertibleString(Encoding.UTF8.GetBytes(utf8String));
    }

    public static ConvertibleString FromByteArray(byte[] bytes)
    {
        return new ConvertibleString(bytes);
    }

    private ConvertibleString(byte[] bytes)
    {
        BytesRepresentation = bytes;
        Base64Representation = Convert.ToBase64String(bytes);
        Utf8Representation = Encoding.UTF8.GetString(bytes);
    }

    private ConvertibleString(string base64String)
    {
        BytesRepresentation = Convert.FromBase64String(base64String);
        Base64Representation = base64String;
        Utf8Representation = Encoding.UTF8.GetString(BytesRepresentation);
    }

    public bool IsEmpty()
    {
        return SizeInBytes == 0;
    }

    protected bool Equals(ConvertibleString? other)
    {
        return string.Equals(Base64Representation, other?.Base64Representation) &&
               string.Equals(Utf8Representation, other?.Utf8Representation);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ConvertibleString)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Base64Representation.GetHashCode();
            hashCode = (hashCode * 397) ^ Utf8Representation.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(ConvertibleString? a, ConvertibleString? b)
    {
        return a?.Equals(b) ?? ReferenceEquals(b, null);
    }

    public static bool operator !=(ConvertibleString? a, ConvertibleString? b)
    {
        if (ReferenceEquals(a, null))
        {
            return !ReferenceEquals(b, null);
        }

        return !a.Equals(b);
    }
}
