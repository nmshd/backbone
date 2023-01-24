namespace Synchronization.Application;

public class ApplicationOptions
{
    public PaginationOptions Pagination { get; set; } = new();
    public ValidationOptions Validation { get; set; } = new();
}

public class PaginationOptions
{
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}

public class ValidationOptions
{
    public int MaxDatawalletModificationPayloadSize { get; set; } = 1000;
}
