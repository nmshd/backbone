using System.CommandLine;
using System.IO.Compression;
using System.Text;
using Spectre.Console;

namespace Backbone.IdentityDeletionVerifier.Commands;

public class CheckCommand : Command
{
    public CheckCommand() : base("check", "Check the exported database file for the given identity address in the temp directory")
    {
        SetAction(Handle);
    }

    private async Task<int> Handle(ParseResult _, CancellationToken cancellationToken)
    {
        if (!DirectoryExists())
        {
            AnsiConsole.MarkupLineInterpolated($"[red bold]The temp directory[/][grey bold]{FilePaths.PATH_TO_TEMP_DIR} [/][red bold]doesn't exist.[/]");
            return -1;
        }

        if (!IdentitiesFileExists())
        {
            AnsiConsole.MarkupLine("[red bold]The deleted identities file doesn't exist. Run the Init command first.[/]");
            return -1;
        }

        if (!ExportFileExists())
        {
            AnsiConsole.MarkupLine("[red bold]No exported database file found. Run the Admin Cli Export Database command first.[/]");
            return -1;
        }

        var a = await GetIdentityToCheck();
        if (a == null)
        {
            AnsiConsole.MarkupLineInterpolated($"[red bold]The identities file couldn't be read or has no Identity[/]");
            return -1;
        }

        AnsiConsole.MarkupLineInterpolated($"[green bold]Identity to check:[/] [grey bold]{a}[/]");

        return await CheckExportFileForIdentities(GetLatestExportFile(), a);
    }

    private bool DirectoryExists() => Directory.Exists(FilePaths.PATH_TO_TEMP_DIR);
    private bool IdentitiesFileExists() => File.Exists(FilePaths.PATH_TO_IDENTITIES_FILE);
    private bool ExportFileExists() => Directory.EnumerateFiles(FilePaths.PATH_TO_TEMP_DIR).Any(FilePaths.EXPORT_FILE_PATTERN.IsMatch);
    private string GetLatestExportFile() => Directory.EnumerateFiles(FilePaths.PATH_TO_TEMP_DIR).Where(e => FilePaths.EXPORT_FILE_PATTERN.IsMatch(e)).Max()!;

    private async Task<string?> GetIdentityToCheck()
    {
        using var reader = new StreamReader(File.OpenRead(FilePaths.PATH_TO_IDENTITIES_FILE), Encoding.UTF8);

        return await reader.ReadLineAsync();
    }

    private async Task<int> CheckExportFileForIdentities(string exportFile, string identityToCheck)
    {
        return await AnsiConsole.Progress()
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(Spinner.Known.Clock)
            )
            .StartAsync(async ctx =>
            {
                using var archive = ZipFile.OpenRead(exportFile);

                var found = 0;
                var tasks = archive.Entries
                    .Select(e => ctx.AddTask(e.Name, autoStart: false))
                    .ToList();

                foreach (var (index, entry) in archive.Entries.Index())
                {
                    var task = tasks[index];
                    task.StartTask();
                    found += await CheckCsvFileForIdentity(entry, identityToCheck, task);
                }

                return found;
            });
    }

    private async Task<int> CheckCsvFileForIdentity(ZipArchiveEntry file, string identityToCheck, ProgressTask progressReporter)
    {
        using var reader = new StreamReader(file.Open(), Encoding.UTF8);
        List<string> lines = [];
        var found = 0;

        while (!reader.EndOfStream)
            lines.Add(await reader.ReadLineAsync() ?? string.Empty);

        progressReporter.MaxValue = lines.Count;

        foreach (var line in lines)
        {
            var count = line
                .Split(',')
                .Count(s => string.Equals(s, identityToCheck, StringComparison.OrdinalIgnoreCase));

            if (count != 0)
                AnsiConsole.MarkupLineInterpolated($"[red bold]Found[/] [grey bold]{count}[/] [red bold]occurrences in[/] [grey bold]{file.Name}[/][red bold]:[/] [grey bold]{line}[/]");

            found += count;
            progressReporter.Increment(1);
        }

        return found;
    }
}
