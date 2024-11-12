using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class ExcelWriter : IExcelWriter
{
    public async Task Write<T>(string filePath, IEnumerable<T> data)
    {
        var excelMapper = new ExcelMapper();
        await excelMapper.SaveAsync(filePath, data);
    }
}
