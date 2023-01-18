namespace Relationships.Application;

public class ApplicationOptions
{
    public PaginationOptions Pagination { get; set; } = new();
}

public class PaginationOptions
{
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}
