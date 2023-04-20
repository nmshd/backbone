using System.Xml.Serialization;

namespace SpecFlowCucumberResultsExporter.Model;

public class TableParam
{
    public List<string> Columns { get; set; }

    [XmlIgnore]
    public List<Dictionary<string, string>> Rows { get; set; }

    public int GetMaxColumnCharacters(int columnIndex)
    {
        int result = 0;
        foreach (Dictionary<string, string> row in Rows)
        {
            foreach (string value in row.Values)
            {
                if (value.Length > result)
                {
                    result = value.Length;
                }
            }
        }
        return result;
    }
}