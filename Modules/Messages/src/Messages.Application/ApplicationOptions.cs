namespace Messages.Application;

public class ApplicationOptions
{
    public int MaxNumberOfUnreceivedMessagesFromOneSender { get; set; }
    public PaginationOptions Pagination { get; set; } = new();
}

public class PaginationOptions
{
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}
