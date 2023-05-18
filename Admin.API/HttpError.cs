using Enmeshed.BuildingBlocks.Domain;

namespace Admin.API;

public class HttpError
{
    protected HttpError(string code, string message, string docs, dynamic data)
    {
        Id = HttpErrorId.New().ToString();
        Code = code;
        Message = message;
        Docs = docs;
        Time = DateTime.UtcNow;
        Data = data;
    }

    public string Id { get; }
    public string Code { get; }
    public string Message { get; }
    public string Docs { get; }
    public DateTime Time { get; }
    public dynamic Data { get; }

    public static HttpError ForProduction(string code, string message, string docs, dynamic data)
    {
        return new HttpErrorProd(code, message, docs, data);
    }

    public static HttpError ForDev(string code, string message, string docs, dynamic data, IEnumerable<string> stacktrace,
        string details)
    {
        return new HttpErrorDev(code, message, docs, data, stacktrace, details);
    }
}

public class HttpErrorProd : HttpError
{
    public HttpErrorProd(string code, string message, string docs, dynamic data) : base(code, message, docs, (object)data)
    {
    }
}

public class HttpErrorDev : HttpError
{
    internal HttpErrorDev(string code, string message, string docs, dynamic data, IEnumerable<string> stacktrace, string details)
        : base(code, message, docs, (object)data)
    {
        Stacktrace = stacktrace;
        Details = details;
    }

    public IEnumerable<string> Stacktrace { get; }
    public string Details { get; }
}

//public class HttpErrorConverter : JsonConverter<HttpError>
//{
//    public override bool CanConvert(Type type)
//    {
//        return typeof(HttpError).IsAssignableFrom(type);
//    }

//    public override HttpError Read(
//        ref Utf8JsonReader reader,
//        Type typeToConvert,
//        JsonSerializerOptions options)
//    {
//        throw new NotImplementedException();
//    }

//    public override void Write(
//        Utf8JsonWriter writer,
//        HttpError value,
//        JsonSerializerOptions options)
//    {
//        if (value is HttpErrorDev devError)
//            JsonSerializer.Serialize(writer, devError, options);
//        else if (value is HttpErrorProd prodError) JsonSerializer.Serialize(writer, prodError, options);
//    }
//}

[Serializable]
public struct HttpErrorId
{
    public const int MAX_LENGTH = 20;
    private static readonly int MaxLengthWithoutPrefix = MAX_LENGTH - PREFIX.Length;
    private const string PREFIX = "ERR";

    private static readonly char[] ValidChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U',
        'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    private HttpErrorId(string stringValue)
    {
        StringValue = stringValue;
    }

    public string StringValue { get; }

    public static HttpErrorId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(HttpErrorId)}.");

        return new HttpErrorId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        if (stringValue == null) return false;

        var hasPrefix = stringValue.StartsWith(PREFIX);
        var lengthIsValid = stringValue.Length <= MAX_LENGTH;
        var hasOnlyValidChars = stringValue.ContainsOnly(ValidChars);

        return hasPrefix && lengthIsValid && hasOnlyValidChars;
    }

    public static HttpErrorId New()
    {
        var tokenIdAsString = StringUtils.Generate(ValidChars, MaxLengthWithoutPrefix);
        return new HttpErrorId(PREFIX + tokenIdAsString);
    }

    public override string ToString()
    {
        return StringValue;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return StringValue;
    }
}