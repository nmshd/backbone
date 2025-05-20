using System.CommandLine;

namespace Backbone.AdminCli.Commands.Database;

public class DatabaseCommand : Command
{
    public DatabaseCommand(ExportDatabaseCommand exportDatabaseCommand) : base("database")
    {
        AddCommand(exportDatabaseCommand);
    }
}

public static class Extensions
{
    public static string ToCsv(this object obj)
    {
        static string? ObjToString(object? obj)
        {
            if (obj?.GetType() == typeof(byte[]))
                return Convert.ToBase64String((byte[])obj);

            return obj?.ToString();
        }

        var properties = obj.GetType().GetProperties();
        var values = properties.Select(p => ObjToString(p.GetValue(obj)) ?? string.Empty);
        return string.Join(",", values);
    }
}
