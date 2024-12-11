using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

public interface IExcelReader
{
    Task<IEnumerable<dynamic>> FetchAsync(string workSheet, ExcelMapper excelMapper, FileStream stream);
}
