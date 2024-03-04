using Backbone.SpecFlowCucumberResultsExporter.Model;

namespace Backbone.SpecFlowCucumberResultsExporter.Reporting;

public abstract class Reporter
{
    public Feature CurrentFeature { get; internal set; }

    public Scenario CurrentScenario { get; internal set; }

    public Step CurrentStep { get; internal set; }

    public virtual string Name => GetType().FullName!;

    public Report Report { get; set; }

    public virtual void WriteToFile(string path)
    {
        using var memoryStream = new MemoryStream();
        WriteToStream(memoryStream);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        using var destination = File.Create(path);
        memoryStream.CopyTo(destination);
    }

    public abstract void WriteToStream(Stream stream);

    public string WriteToString()
    {
        using var memoryStream = new MemoryStream();
        WriteToStream(memoryStream);
        memoryStream.Position = 0L;
        using var streamReader = new StreamReader(memoryStream);
        return streamReader.ReadToEnd();
    }
}
