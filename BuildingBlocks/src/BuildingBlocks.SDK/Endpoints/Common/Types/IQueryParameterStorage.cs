using System.Collections.Specialized;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

public interface IQueryParameterStorage
{
    NameValueCollection ToQueryParameters();
}
