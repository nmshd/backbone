using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SpecFlowCucumberResultsExporter.Extensions;

public class ReportContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

        foreach (JsonProperty property in properties)
        {
            property.PropertyName = ConvertPropertyName(property.PropertyName);
        }

        // only seria
        return properties;
    }

    private string ConvertPropertyName(string name)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    result.Append('_');
                }
                result.Append(c.ToString().ToLower());
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}
