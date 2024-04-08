using System.Collections.Specialized;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public interface IQueryParameterStorage
{
    public NameValueCollection ToQueryParameters();
}
