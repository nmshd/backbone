namespace Backbone.BuildingBlocks.API;

public class HttpResponseEnvelope
{
    public static HttpResponseEnvelope CreateError(HttpError error)
    {
        return new HttpResponseEnvelopeError(error);
    }

    public static HttpResponseEnvelope CreateSuccess()
    {
        return new HttpResponseEnvelope();
    }

    public static HttpResponseEnvelopeResult<T> CreateSuccess<T>(T result)
    {
        return new HttpResponseEnvelopeResult<T>(result);
    }
}

public class HttpResponseEnvelopeResult<T> : HttpResponseEnvelope
{
    public HttpResponseEnvelopeResult(T result)
    {
        Result = result;
    }

    public T Result { get; set; }
}

public class PagedHttpResponseEnvelope<T> : HttpResponseEnvelopeResult<IEnumerable<T>>
{
    public PagedHttpResponseEnvelope(IEnumerable<T> result, PaginationData paginationData) : base(result)
    {
        Pagination = paginationData;
    }

    public PaginationData Pagination { get; set; }

    public class PaginationData
    {
        public int PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}

public class HttpResponseEnvelopeError : HttpResponseEnvelope
{
    public HttpResponseEnvelopeError(HttpError error)
    {
        Error = error;
    }

    public HttpError Error { get; set; }
}
