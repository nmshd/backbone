using System.Text;
using Backbone.SpecFlowCucumberResultsExporter.Reporting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backbone.SpecFlowCucumberResultsExporter.Extensions;

public class JsonReporter : Reporter
{
    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    public JsonReporter()
    {
        JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new ReportContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = Enumerable.ToList(new JsonConverter[1]
            {
                new StringEnumConverter()
            })
        };
    }

    public override void WriteToStream(Stream stream)
    {
        var s = JsonConvert.SerializeObject(base.Report.Features, JsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(s);
        using var memoryStream = new MemoryStream(bytes);
        memoryStream.CopyTo(stream);
    }
}
