using Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Enmeshed.BuildingBlocks.Application.Pagination
{
    public class PagedResponse<T> : EnumerableResponseBase<T>
    {
        public PagedResponse(IEnumerable<T> data, PaginationFilter previousFilter, int totalRecords) : base(data)
        {
            Pagination = new PaginationData(previousFilter, totalRecords);
        }

        public PaginationData Pagination { get; set; }
    }

    public class PaginationData
    {
        public PaginationData(PaginationFilter previousFilter, int totalRecords)
        {
            TotalRecords = totalRecords;

            var pageSize = previousFilter.PageSize ?? totalRecords;
            var totalPages = (double) totalRecords / pageSize;
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            PageNumber = previousFilter.PageNumber;
            PageSize = pageSize;
            TotalPages = roundedTotalPages;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}