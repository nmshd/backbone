using SpecFlowCucumberResultsExporter.Model;

namespace SpecFlowCucumberResultsExporter.Reporting;

public abstract class Reporter
{
    public Feature CurrentFeature { get; internal set; }

    public Scenario CurrentScenario { get; internal set; }

    public Step CurrentStep { get; internal set; }

    public virtual string Name => GetType().FullName;

    public Report Report { get; set; }

    public virtual void WriteToFile(string path)
    {
        using MemoryStream memoryStream = new MemoryStream();
        WriteToStream(memoryStream);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        using FileStream destination = File.Create(path);
        memoryStream.CopyTo(destination);
    }

    public abstract void WriteToStream(Stream stream);

    public string WriteToString()
    {
        using MemoryStream memoryStream = new MemoryStream();
        WriteToStream(memoryStream);
        memoryStream.Position = 0L;
        using StreamReader streamReader = new StreamReader(memoryStream);
        return streamReader.ReadToEnd();
    }
}
