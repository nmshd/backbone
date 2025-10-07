using Asp.Versioning;

namespace Backbone.AdminApi.Versions;

public class V1Attribute : ApiVersionAttribute
{
    public V1Attribute() : base("1")
    {
    }
}
