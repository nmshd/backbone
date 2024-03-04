namespace Backbone.Modules.Devices.AdminCli;

public static class ConsoleHelpers
{
    public static string ReadRequired(string hint, Func<string?, string>? isValid = null)
    {
        return Read(hint, isValid)!;
    }

    public static string? ReadOptional(string hint, Func<string?, string>? isValid = null)
    {
        return Read(hint, isValid, true);
    }

    public static string? Read(string hint, Func<string?, string>? validateValue = null, bool optional = false)
    {
        while (true)
        {
            Console.Write(hint + ": ");

            var value = Console.ReadLine();

            if (string.IsNullOrEmpty(value))
            {
                if (optional)
                    return value;

                Console.Error.WriteLine("The value cannot be empty.");
                continue;
            }

            if (validateValue != null)
            {
                var err = validateValue(value);
                if (!string.IsNullOrEmpty(err))
                {
                    Console.Error.WriteLine(err);
                    continue;
                }
            }

            return value;
        }
    }

    public static int ReadRequiredNumber(string helpText, int min, int max)
    {
        helpText = helpText.Replace("{min}", min.ToString()).Replace("{max}", max.ToString());
        var input = ReadRequired(helpText, value =>
        {
            var isInt = int.TryParse(value, out var inputAsInt);

            if (!isInt || inputAsInt < min || inputAsInt > max)
                return $"The value has to be between {min} and {max}";

            return "";
        });

        return int.Parse(input);
    }

    public static int? ReadOptionalNumber(string helpText, int? min, int? max)
    {
        helpText = helpText.Replace("{min}", min.ToString()).Replace("{max}", max.ToString());
        var input = ReadOptional(helpText, value =>
        {
            var isInt = int.TryParse(value, out var inputAsInt);

            if (!isInt || inputAsInt < min || inputAsInt > max)
                return $"The value has to be between {min} and {max}";

            return "";
        });

        if (string.IsNullOrEmpty(input))
            return null;

        return int.Parse(input);
    }
}
