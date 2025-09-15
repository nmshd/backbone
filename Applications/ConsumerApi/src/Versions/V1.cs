using Asp.Versioning;

namespace Backbone.ConsumerApi.Versions;

public class V1Attribute : ApiVersionAttribute
{
    public V1Attribute() : base("1")
    {
    }
}

public class MapToV1Attribute : MapToApiVersionAttribute
{
    public MapToV1Attribute() : base("1")
    {
    }
}
