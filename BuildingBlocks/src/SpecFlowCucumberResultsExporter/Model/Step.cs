using Newtonsoft.Json;

namespace Backbone.SpecFlowCucumberResultsExporter.Model;

public class Step : ReportItem
{
    public string MultiLineParameter { get; set; }
    public ExceptionInfo Exception { get; set; }
    public new StepResult Result { get; set; }
    public List<Row> Rows { get; set; }

    [JsonProperty("embeddings")]
    public List<Embedding> Embeddings { get; set; } = [];

    public void AddEmbedding(string mimeType, string base64data)
    {
        Embeddings.Add(new Embedding(mimeType, base64data));
    }
}
