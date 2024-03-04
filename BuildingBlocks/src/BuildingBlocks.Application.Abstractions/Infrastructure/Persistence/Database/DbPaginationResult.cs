namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;

public class DbPaginationResult<T>
{
    public DbPaginationResult(IEnumerable<T> itemsOnPage, int totalNumberOfItems)
    {
        ItemsOnPage = itemsOnPage;
        TotalNumberOfItems = totalNumberOfItems;
    }

    public IEnumerable<T> ItemsOnPage { get; set; }
    public int TotalNumberOfItems { get; set; }
}
