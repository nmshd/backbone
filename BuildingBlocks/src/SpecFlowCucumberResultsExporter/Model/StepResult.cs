using Newtonsoft.Json;

namespace SpecFlowCucumberResultsExporter.Model
{
    public class StepResult
    {
        public long Duration { get; set; }

        public TestResult Status { get; set; }

        [JsonProperty("error_message")]
        public string Error { get; set; }
    }
}