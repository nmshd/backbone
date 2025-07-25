﻿using System.CommandLine;
using System.Text.Json;
using Backbone.AdminCli.Commands.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.AdminCli.Commands.Announcements;

public class SendAnnouncementCommand : AdminCliCommand
{
    public SendAnnouncementCommand(IMediator mediator) : base(mediator, "send")
    {
        var expiresAt = new Option<string?>("--expiration")
        {
            Required = false,
            Description = "The expiration date of the announcement."
        };

        var severity = new Option<string?>("--severity")
        {
            Required = true,
            Description = "The severity of the announcement. Possible values: Low, Medium, High"
        };

        var isSilent = new Option<bool?>("--silent")
        {
            Required = false,
            Description = "Whether the announcement should be silent. A push notification will not be sent for silent announcements. By default, the announcement is not silent."
        };

        var iqlQuery = new Option<string?>("--iql-query")
        {
            Required = false,
            Description = "The IQL query that must be matched by receiving identities in order to show the announcement."
        };

        var recipients = new Option<IEnumerable<string>>("--recipients")
        {
            Required = false,
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ZeroOrMore,
            DefaultValueFactory = _ => [],
            Description = "The recipients of the announcement (separated by a whitespace i.e. \"--recipients addr1 addr2 addr3\"). If not specified, the announcement will be sent to all identities."
        };

        Options.Add(expiresAt);
        Options.Add(severity);
        Options.Add(isSilent);
        Options.Add(iqlQuery);
        Options.Add(recipients);

        SetAction((parseResult, cancellationToken) =>
        {
            var severityValue = parseResult.GetRequiredValue(severity);
            var expiresAtValue = parseResult.GetValue(expiresAt);
            var isSilentValue = parseResult.GetValue(isSilent);
            var iqlQueryValue = parseResult.GetValue(iqlQuery);
            var recipientsValue = parseResult.GetValue(recipients);

            return SendAnnouncement(severityValue, expiresAtValue, isSilentValue, iqlQueryValue, [.. recipientsValue!], cancellationToken);
        });
    }

    private async Task<int> SendAnnouncement(string? severityInput, string? expiresAtInput, bool? isSilent, string? iqlQuery, List<string> recipients, CancellationToken cancellationToken)
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

            // if the --recipients option is empty, another flag could be parsed as the first result i.e. "--silent"
            if (recipients.Count == 0)
            {
                throw new Exception("If you use the \"--recipients\" options, at least one recipient has to be specified.");
            }

            var texts = ReadTextsFromCommandLineInput();
            if (texts.Count == 0)
                throw new Exception(@"No texts provided. Exiting...");

            Console.WriteLine(@"You entered the following texts:");
            Console.WriteLine(JsonSerializer.Serialize(texts, JSON_SERIALIZER_OPTIONS));

            if (!PromptForConfirmation(@"Do you want to proceed?"))
            {
                Console.WriteLine(@"The operation was cancelled.");
                return 0;
            }

            Console.WriteLine(@"Sending announcement...");

            var response = await _mediator.Send(new CreateAnnouncementCommand
            {
                Texts = texts,
                Severity = severity,
                IsSilent = isSilent ?? false,
                ExpiresAt = expiresAt,
                Actions = [],
                IqlQuery = iqlQuery,
                Recipients = recipients
            }, cancellationToken);

            Console.WriteLine(@"Announcement sent successfully");
            Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));

            return 0;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($@"An error occurred: {e.Message}");
            return 1;
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

        while (PromptForConfirmation(@"Do you want to add another language?"))
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

            if (string.IsNullOrWhiteSpace(input))
                continue;
            if (input.Trim().Equals("x", StringComparison.CurrentCultureIgnoreCase))
                return null;

            break;
        }

        return input;
    }

    private static bool PromptForConfirmation(string prompt)
    {
        var input = PromptForInput($"{prompt} ([y]es/[N]o): ", true);
        return input?.Trim().ToLower() is "yes" or "y";
    }
}
