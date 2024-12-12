using System.Diagnostics;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class ProcessHelper : IProcessHelper
{
    public async Task<DatabaseRestoreResult> ExecuteProcess(string command, Predicate<ProcessParams> processPredicate)
    {
        var psi = new ProcessStartInfo("cmd", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        var output = await process!.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        var errorMessage = GetErrorMessage(error);
        var hasError = !string.IsNullOrEmpty(errorMessage);

        var status = processPredicate(new ProcessParams(process, output, hasError));

        return new DatabaseRestoreResult(status, $"{output} {errorMessage}", IsError: hasError);
    }

    private static string GetErrorMessage(string error)
    {
        return !string.IsNullOrWhiteSpace(error)
            ? error.StartsWith("psql: error: connection to server", StringComparison.OrdinalIgnoreCase)
                ? $"Is enmeshed backbone postgres container running?{Environment.NewLine}{ERROR}: {error}"
                : $"{Environment.NewLine}{ERROR}:{error}"
            : string.Empty;
    }
}
