using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Application.Housekeeping;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 769524,
        EventName = "ExecuteHousekeeping.DataDeleted",
        Level = LogLevel.Information,
        Message = "Deleted {numberOfDeletedItems} {descriptionOfDeletedObjects}.")]
    public static partial void DataDeleted(this ILogger logger, int numberOfDeletedItems, string descriptionOfDeletedObjects);
}
