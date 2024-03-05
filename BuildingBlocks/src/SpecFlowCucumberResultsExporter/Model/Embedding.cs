using Newtonsoft.Json;

namespace Backbone.SpecFlowCucumberResultsExporter.Model;

public class Embedding
{
    public Embedding(string mimeType, string base64data)
    {
        MimeType = mimeType;
        Base64Data = base64data;
    }

    [JsonProperty("mime_type")]
    public string MimeType { get; set; }

    [JsonProperty("data")]
    public string Base64Data { get; set; }
}
