namespace Backbone.SpecFlowCucumberResultsExporter.Model;

public class Feature : TaggedReportItem
{
    private string _description;

    private static readonly string DEFAULT_URI = "/uri/placeholder";

    public new string Description
    {
        get => _description;
        set => _description = string.IsNullOrEmpty(value) ? value : value.Replace("\r", "");
    }
    public string Uri { get; set; } = DEFAULT_URI;

    public List<Scenario> Elements { get; set; }
    public new string Keyword => "Feature";
}
