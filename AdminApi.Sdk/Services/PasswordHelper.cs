using System.Text;

namespace Backbone.AdminApi.Sdk.Services;

public class PasswordHelper
{
    public static string GeneratePassword(int minLength, int maxLength)
    {
        // ReSharper disable StringLiteralTypo
        var specialCharacterBucket = new Bucket { MinLength = 1, MaxLength = 1, AllowedChars = "!?-_.:,;#+" };
        var lowercaseBucket = new Bucket { MinLength = 1, MaxLength = 1, AllowedChars = "abcdefghijkmnpqrstuvwxyz" };
        var uppercaseBucket = new Bucket { MinLength = 1, MaxLength = 1, AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ" };
        var numberBucket = new Bucket { MinLength = 1, MaxLength = 1, AllowedChars = "123456789" };
        var alphanumericBucket = new Bucket { MinLength = minLength - 4, MaxLength = maxLength - 4, AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" };
        // ReSharper enable StringLiteralTypo

        var password = RandomStringWithBuckets(specialCharacterBucket, lowercaseBucket, uppercaseBucket, numberBucket, alphanumericBucket);
        return Scramble(password);
    }

    private static string RandomStringWithBuckets(params Bucket[] buckets)
    {
        var random = new Random();
        var stringBuilder = new StringBuilder();
        foreach (var bucket in buckets)
        {
            var length = random.Next(bucket.MinLength, bucket.MaxLength + 1);
            for (var i = 0; i < length; i++)
            {
                var index = random.Next(0, bucket.AllowedChars.Length);
                stringBuilder.Append(bucket.AllowedChars[index]);
            }
        }

        return stringBuilder.ToString();
    }

    private static string Scramble(string input)
    {
        var charArray = input.ToCharArray();
        var random = new Random();
        for (var i = 0; i < charArray.Length; i++)
        {
            var j = random.Next(i, charArray.Length);
            (charArray[i], charArray[j]) = (charArray[j], charArray[i]);
        }

        return new string(charArray);
    }

    private class Bucket
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public string AllowedChars { get; set; }
    }
}
