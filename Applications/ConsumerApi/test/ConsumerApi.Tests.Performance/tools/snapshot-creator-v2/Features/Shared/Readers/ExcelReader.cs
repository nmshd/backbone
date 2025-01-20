using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

public class ExcelReader : IExcelReader
{
    public Task<IEnumerable<dynamic>> FetchAsync(string workSheet, ExcelMapper excelMapper, FileStream stream) => excelMapper.FetchAsync(stream, workSheet);
}
