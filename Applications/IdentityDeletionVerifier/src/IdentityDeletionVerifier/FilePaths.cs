using System.Text.RegularExpressions;

namespace Backbone.IdentityDeletionVerifier;

public static partial class FilePaths
{
    public static readonly string PATH_TO_TEMP_DIR = Path.Combine(Path.GetTempPath(), "enmeshed", "backbone");
    public const string IDENTITIES_FILENAME = "deleted-identities.txt";
    public static readonly Regex EXPORT_FILE_PATTERN = MyRegex();
    public static readonly string PATH_TO_IDENTITIES_FILE = Path.Combine(PATH_TO_TEMP_DIR, IDENTITIES_FILENAME);

    [GeneratedRegex(@"export-\d{8}_\d{6}\.zip$")]
    private static partial Regex MyRegex();
}
