using System.Collections.Specialized;

namespace Backbone.AdminApi.Sdk.Endpoints.Common.Types;

public interface IQueryParameterStorage
{
    public NameValueCollection ToQueryParameters();
}
