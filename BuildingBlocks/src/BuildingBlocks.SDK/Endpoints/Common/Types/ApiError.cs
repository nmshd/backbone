using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public class ApiError
{
    public required string Id { get; set; }
    public required string Code { get; set; }
    public required string Message { get; set; }
    public required string Docs { get; set; }
    public required DateTime Time { get; set; }
    public ApiErrorData? Data { get; set; }
}

public class ApiErrorData
{
    private readonly JsonElement _data;
    private readonly JsonSerializerOptions _optionsUsedToDeserializeThis;

    private ApiErrorData(JsonElement data, JsonSerializerOptions options)
    {
        _data = data;
        _optionsUsedToDeserializeThis = options; // we need to keep the options to be able to deserialize the data later
    }

    public T As<T>()
    {
        var json = _data.GetRawText();
        return JsonSerializer.Deserialize<T>(json, _optionsUsedToDeserializeThis)!;
    }

    public class JsonConverter : JsonConverter<ApiErrorData>
    {
        public override ApiErrorData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var element = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            return new ApiErrorData(element, options);
        }

        public override void Write(Utf8JsonWriter writer, ApiErrorData value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.As<JsonElement>(), options);
        }
    }
}
