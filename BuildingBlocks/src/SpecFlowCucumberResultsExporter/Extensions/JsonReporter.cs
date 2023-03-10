using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.Extensions;

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
        string s = JsonConvert.SerializeObject(base.Report.Features, JsonSerializerSettings);
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        using MemoryStream memoryStream = new MemoryStream(bytes);
        memoryStream.CopyTo(stream);
    }
}
