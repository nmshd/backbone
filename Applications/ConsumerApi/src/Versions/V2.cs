using Asp.Versioning;

namespace Backbone.ConsumerApi.Versions;

public class V2Attribute : ApiVersionAttribute
{
    public V2Attribute() : base("2")
    {
    }
}

public class MapToV2Attribute : MapToApiVersionAttribute
{
    public MapToV2Attribute() : base("2")
    {
    }
}
