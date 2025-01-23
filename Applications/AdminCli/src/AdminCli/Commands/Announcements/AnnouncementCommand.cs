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
            Description = "The severity of the announcement."
        };

        AddOption(expiresAt);
        AddOption(severity);


        this.SetHandler(SendAnnouncement, DB_PROVIDER_OPTION, DB_CONNECTION_STRING_OPTION, severity, expiresAt);
    }

    private async Task SendAnnouncement(string dbProvider, string dbConnectionString, string? severityInput, string? expiresAtInput)
    {
        var severity = severityInput switch
        {
            _ when string.IsNullOrWhiteSpace(severityInput) => AnnouncementSeverity.Low,
            _ when Enum.TryParse<AnnouncementSeverity>(severityInput, ignoreCase: true, out var parsedSeverity) => parsedSeverity,
            _ => throw new ArgumentException($@"Specified severity '{severityInput}' is not a valid severity.")
        };

        DateTime? expiresAt = expiresAtInput switch
        {
            _ when string.IsNullOrWhiteSpace(expiresAtInput) => null,
            _ when DateTime.TryParse(expiresAtInput, out var parsedDateTime) => parsedDateTime,
            _ => throw new ArgumentException($@"Specified expiration datetime '{expiresAtInput}' is not a valid DateTime.")
        };

        var mediator = _serviceLocator.GetService<IMediator>(dbProvider, dbConnectionString);

        var response = await mediator.Send(new CreateAnnouncementCommand
        {
            Texts = null!,
            Severity = severity,
            ExpiresAt = expiresAt
        }, CancellationToken.None);

        Console.WriteLine(@"Announcement sent successfully");

        Console.WriteLine(JsonSerializer.Serialize(response, JSON_SERIALIZER_OPTIONS));
    }
}
