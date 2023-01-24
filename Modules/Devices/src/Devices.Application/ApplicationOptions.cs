namespace Devices.Application;

public class ApplicationOptions
{
    public string AddressPrefix { get; set; }
    public PaginationOptions Pagination { get; set; } = new();
}

public class PaginationOptions
{
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}