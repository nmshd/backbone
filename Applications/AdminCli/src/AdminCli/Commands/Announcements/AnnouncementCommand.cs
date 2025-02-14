using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.AdminCli.Commands.Announcements;

public class AnnouncementCommand : AdminCliCommand
{
    public AnnouncementCommand(ServiceLocator serviceLocator) : base("announcement", serviceLocator)
    {
        AddCommand(new SendAnnouncementCommand(serviceLocator));
    }
}

public class SendAnnouncementCommand : AdminCliDbCommand
{
    public SendAnnouncementCommand(ServiceLocator serviceLocator) : base("send", serviceLocator)
    {
        var expiresAt = new Option<string?>("--expiration")
        {
            IsRequired = false,
            Description = "The expiration date of the announcement."
        };

        var severity = new Option<string?>("--severity")
        {
            IsRequired = true,
            Description = "The severity of the announcement. Possible values: Low, Medium, High"
        };

        AddOption(expiresAt);
        AddOption(severity);

        this.SetHandler(SendAnnouncement, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, severity, expiresAt);
    }

    private async Task SendAnnouncement(string dbProvider, string dbConnectionString, string? severityInput, string? expiresAtInput)
    {
        try
        {
            var severity = severityInput switch
            {
                _ when Enum.TryParse<AnnouncementSeverity>(severityInput, ignoreCase: true, out var parsedSeverity) => parsedSeverity,
                _ => throw new ArgumentException($@"Specified severity '{severityInput}' is not a valid severity.")
            };

            DateTime? expiresAt = expiresAtInput switch
            {
                _ when string.IsNullOrWhiteSpace(expiresAtInput) => null,
                _ when DateTime.TryParse(expiresAtInput, out var parsedDateTime) => parsedDateTime,
                _ => throw new ArgumentException($@"Specified expiration datetime '{expiresAtInput}' is not a valid DateTime.")
            };

            var texts = ReadTextsFromCommandLineInput();

            if (texts.Count == 0)
            {
                Console.WriteLine(@"No texts provided. Exiting...");
                return;
            }

            Console.WriteLine(@"You entered the following texts:");
            Console.WriteLine(JsonSerializer.Serialize(texts, JSON_SERIALIZER_OPTIONS));
            if (!PromptForConfirmation(@"Do you want to proceed? ([y]es/[N]o): ")) return;

            Console.WriteLine(@"Sending announcement...");

            var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

            var response = await mediator.Send(new CreateAnnouncementCommand
            {
                Texts = texts,
                Severity = severity,
                ExpiresAt = expiresAt
            }, CancellationToken.None);

            Console.WriteLine(@"Announcement sent successfully");
            Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));
        }
        catch (Exception e)
        {
            Console.WriteLine($@"An error occurred: {e.Message}");
        }
    }

    private static List<CreateAnnouncementCommandText> ReadTextsFromCommandLineInput()
    {
        var texts = new List<CreateAnnouncementCommandText>();

        var englishTitle = PromptForInput(@"Enter english title: ");
        var englishBody = PromptForInput(@"Enter english body: ");

        if (englishTitle == null || englishBody == null) return texts;

        texts.Add(new CreateAnnouncementCommandText
        {
            Language = "en",
            Title = englishTitle,
            Body = englishBody
        });

        while (PromptForConfirmation(@"Do you want to add another language? ([y]es/[N]o): "))
        {
            var language = PromptForInput(@"Enter a two-letter language code (e.g. de, it, nl): ");
            var title = PromptForInput(@"Enter title: ");
            var body = PromptForInput(@"Enter body: ");

            if (language == null || title == null || body == null)
            {
                break;
            }

            texts.Add(new CreateAnnouncementCommandText
            {
                Language = language,
                Title = title,
                Body = body
            });
        }

        return texts;
    }

    private static string? PromptForInput(string prompt, bool allowEmpty = false)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        while (!allowEmpty && string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(@"Input cannot be empty. Press x to exit.");
            Console.Write(prompt);
            input = Console.ReadLine();

            if (input == null) continue;
            if (input.Trim().Equals("x", StringComparison.CurrentCultureIgnoreCase)) return null;

            break;
        }

        return input;
    }

    private static bool PromptForConfirmation(string prompt)
    {
        var input = PromptForInput(prompt, true);
        return input?.Trim().ToLower() is "yes" or "y";
    }
}
