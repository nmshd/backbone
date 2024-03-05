namespace Backbone.AdminApi.Sdk.Endpoints.Common.Types;

public class AdminApiError
{
    public required string Id { get; set; }
    public required string Code { get; set; }
    public required string Message { get; set; }
    public required string Docs { get; set; }
    public required DateTime Time { get; set; }
}

public class AdminApiError<TData> : AdminApiError
{
    public required TData Data { get; set; }
}
