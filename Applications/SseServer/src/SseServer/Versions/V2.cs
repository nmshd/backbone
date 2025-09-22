using Asp.Versioning;

namespace Backbone.SseServer.Versions;

public class V2Attribute : ApiVersionAttribute
{
    public V2Attribute() : base("2")
    {
    }
}
