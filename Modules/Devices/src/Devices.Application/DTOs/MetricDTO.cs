namespace Backbone.Modules.Devices.Application.DTOs;

public class MetricDTO
{
    public MetricDTO() { }

    public MetricDTO(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public string Key { get; set; }
    public string DisplayName { get; set; }
}
