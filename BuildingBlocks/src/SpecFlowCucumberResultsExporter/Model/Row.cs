namespace Backbone.SpecFlowCucumberResultsExporter.Model;

public class Row
{
    public static readonly int LINE_FILLER = 0;

    public List<string> Cells { get; set; }

    public int Line => LINE_FILLER;
}
