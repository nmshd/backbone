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
        static string ObjToString(object? obj)
        {
            if (obj?.GetType() == typeof(byte[]))
                return Convert.ToBase64String((byte[])obj);

            return obj?.ToString() ?? string.Empty;
        }

        var propertyValuesAsStrings = obj.GetType().GetProperties()
            .Select(propertyInfo => propertyInfo.GetValue(obj))
            .Select(ObjToString);

        return string.Join(",", propertyValuesAsStrings);
    }
}
