using Newtonsoft.Json;

namespace SpecFlowCucumberResultsExporter.Model
{
    public abstract class ReportItem
    {
        public static readonly int LineFiller = 0;
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }
        public virtual TestResult Result { get; set; }
        public int Line => LineFiller;
        public string Keyword { get; set; }

    }
}