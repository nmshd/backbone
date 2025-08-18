using System.CommandLine;
using System.Net;
using System.Text;
using System.Text.Unicode;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.Crypto;
using Backbone.IdentityDeletionVerifier.Extensions;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using Spectre.Console;
using AdminClient = Backbone.AdminApi.Sdk.Client;

namespace Backbone.IdentityDeletionVerifier.Commands;

public class InitCommand : Command
{
    private static readonly ClientCredentials CREDENTIALS = new("test", "test");
    private const string PASSWORD = "password";
    private const string ADMIN_API_KEY = "test";
    private const string DUMMY_STRING = "AAAA";
    private static readonly byte[] DUMMY_DATA = DUMMY_STRING.GetBytes();

    public InitCommand() : base("init", "Creates an identity with relationships, messages etc. and starts an immediate deletion process")
    {
        var consumerBaseUrl = new Option<string>("--consumerBaseUrl")
        {
            Required = true,
            Description = "The base url for the Consumer Api"
        };

        var adminBaseUrl = new Option<string>("--adminBaseUrl")
        {
            Required = true,
            Description = "The base url for the Admin Api"
        };

        Options.Add(consumerBaseUrl);
        Options.Add(adminBaseUrl);
        SetAction((result, _) => Handle(result.GetRequiredValue(consumerBaseUrl), result.GetRequiredValue(adminBaseUrl)));
    }

    private async Task<int> Handle(string consumerBaseUrl, string adminBaseUrl)
    {
        List<bool> results = [];

        var ex = await AnsiConsole.Status()
            .Spinner(Spinner.Known.Clock)
            .StartAsync("Preparing the identity deletion", async _ =>
            {
                try
                {
                    var (a, b, admin) = await CreateClients(consumerBaseUrl, adminBaseUrl);
                    results.Add(await CreateChallenge(a));
                    results.Add(await CreateFile(a));
                    results.Add(await CreatePushNotificationHandle(a));
                    results.Add(await CreateIndividualQuota(a, admin));
                    results.Add(await CreateTokens(a, b));
                    results.Add(await CreateDatawallet(a));
                    results.Add(await StartSyncRun(a));
                    results.Add(await CreateRelationship(a, b));
                    results.Add(await SendMessage(a, b));

                    results.Add(await StartDeletionProcesses(a, b));

                    await WriteIdentitiesToFile(a, b);
                }
                catch (Exception e)
                {
                    AnsiConsole.WriteException(e);
                    return false;
                }

                return true;
            });
        results.Add(ex);

        return results.Contains(false) ? 1 : 0;
    }

    private async Task<(Client, Client, AdminClient)> CreateClients(string consumerBaseUrl, string adminBaseUrl)
    {
        AnsiConsole.WriteLine("Creating clients...");
        var a = await Client.CreateForNewIdentity(consumerBaseUrl, CREDENTIALS, PASSWORD);
        var b = await Client.CreateForNewIdentity(consumerBaseUrl, CREDENTIALS, PASSWORD);
        var admin = AdminClient.Create(adminBaseUrl, ADMIN_API_KEY);

        AnsiConsole.MarkupLineInterpolated($"A: [green bold]{a.IdentityData?.Address}[/]");
        AnsiConsole.MarkupLineInterpolated($"B: [green bold]{b.IdentityData?.Address}[/]");
        AnsiConsole.WriteLine(Emoji.Known.CheckMarkButton);

        return (a, b, admin);
    }

    private async Task<bool> CreateChallenge(Client a)
    {
        AnsiConsole.WriteLine("Creating challenge...");
        var response = await a.Challenges.CreateChallenge();
        return AnsiConsole.Console.WriteResult(response);
    }

    private async Task<bool> CreateFile(Client a)
    {
        AnsiConsole.WriteLine("Creating file...");
        var response = await a.Files.UploadFile(new CreateFileRequest
        {
            Content = new MemoryStream("Content".GetBytes()),
            Owner = a.IdentityData!.Address,
            OwnerSignature = DUMMY_STRING,
            CipherHash = DUMMY_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = DUMMY_STRING
        });
        return AnsiConsole.Console.WriteResult(response);
    }

    private async Task<bool> CreatePushNotificationHandle(Client a)
    {
        AnsiConsole.WriteLine("Creating push notification handle...");
        var response = await a.PushNotifications.RegisterForPushNotifications(new UpdateDeviceRegistrationRequest
        {
            AppId = "de.bildungsraum.wallet.experimental",
            Handle = "asdsdaasdasdasds",
            Platform = "dummy"
        });
        return AnsiConsole.Console.WriteResult(response);
    }

    private async Task<bool> CreateIndividualQuota(Client a, AdminClient admin)
    {
        AnsiConsole.WriteLine("Creating individual quota...");
        var response = await admin.Identities.CreateIndividualQuota(a.IdentityData!.Address, new CreateQuotaForIdentityRequest
        {
            Max = 100,
            MetricKey = "NumberOfSentMessages",
            Period = "hour"
        });
        return AnsiConsole.Console.WriteResult(response);
    }

    private async Task<bool> CreateTokens(Client a, Client b)
    {
        AnsiConsole.WriteLine("Creating tokens (one from A, one for A, one allocated by A)...");
        var tA = await a.Tokens.CreateToken(new CreateTokenRequest
        {
            Content = DUMMY_DATA,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            ForIdentity = null,
            Password = null
        });
        var tForA = await b.Tokens.CreateToken(new CreateTokenRequest
        {
            Content = DUMMY_DATA,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            ForIdentity = a.IdentityData!.Address,
            Password = null
        });
        var tAllocatedByA = await b.Tokens.CreateToken(new CreateTokenRequest
        {
            Content = DUMMY_DATA,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            ForIdentity = a.IdentityData!.Address,
            Password = null
        });

        //var allocationResponse = tAllocatedByA.IsSuccess ? await a.Tokens.GetToken(tAllocatedByA.Result!.Id) : DummyErrorResponse<Token>();

        return AnsiConsole.Console.WriteResult(tA, tForA, tAllocatedByA /*, allocationResponse*/);
    }

    private async Task<bool> CreateDatawallet(Client a)
    {
        AnsiConsole.WriteLine("Creating datawallet...");
        AnsiConsole.WriteLine("Not yet implemented");

        //TODO: Timo (How to create a datawallet?)
        await Task.CompletedTask;

        return true;
    }

    private async Task<bool> StartSyncRun(Client a)
    {
        AnsiConsole.WriteLine("Starting sync run...");
        var response = await a.SyncRuns.StartSyncRun(new StartSyncRunRequest
        {
            Duration = null,
            Type = SyncRunType.ExternalEventSync
        }, 1);
        return AnsiConsole.Console.WriteResult(response);
    }

    private async Task<bool> CreateRelationship(Client a, Client b)
    {
        AnsiConsole.WriteLine("Creating relationship...");
        var templateForA = await b.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = DUMMY_DATA, ForIdentity = a.IdentityData!.Address });
        if (templateForA.IsError) return false;

        var templateByA = await a.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = DUMMY_DATA });

        var templateAllocatedByA = await b.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest { Content = DUMMY_DATA });
        if (templateAllocatedByA.IsError) return false;
        var getTemplateResponse = await a.RelationshipTemplates.GetTemplate(templateAllocatedByA.Result!.Id);

        var createRelationshipResponse = await a.Relationships.CreateRelationship(new CreateRelationshipRequest { Content = DUMMY_DATA, RelationshipTemplateId = templateForA.Result!.Id });
        if (createRelationshipResponse.IsError) return false;

        var acceptRelationshipResponse = await b.Relationships.AcceptRelationship(createRelationshipResponse.Result!.Id, new AcceptRelationshipRequest { CreationResponseContent = DUMMY_DATA });

        return AnsiConsole.Console.WriteResult(templateForA, templateByA, templateAllocatedByA, getTemplateResponse, createRelationshipResponse, acceptRelationshipResponse);
    }

    private async Task<bool> SendMessage(Client a, Client b)
    {
        AnsiConsole.WriteLine("Sending message from A to B...");
        var sendMessageResponse = await a.Messages.SendMessage(new SendMessageRequest
        {
            Attachments = [],
            Body = "Message".GetBytes(),
            Recipients =
            [
                new SendMessageRequestRecipientInformation
                {
                    Address = b.IdentityData!.Address,
                    EncryptedKey = ConvertibleString.FromUtf8("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA").BytesRepresentation
                }
            ]
        });
        return AnsiConsole.Console.WriteResult(sendMessageResponse);
    }

    private async Task<bool> StartDeletionProcesses(Client a, Client b)
    {
        AnsiConsole.WriteLine("Starting deletion processes...");

        var responseA = await a.Identities.StartDeletionProcess(new StartDeletionProcessRequest { LengthOfGracePeriodInDays = 0 });
        var responseB = await b.Identities.StartDeletionProcess(new StartDeletionProcessRequest { LengthOfGracePeriodInDays = 0.1 });
        //TODO Timo: Until the Identity Deletion Bug PR is merged, A and B can't be deleted at the same time (therefore the short grace period)

        return AnsiConsole.Console.WriteResult(responseA, responseB);
    }

    private async Task WriteIdentitiesToFile(Client a, Client b)
    {
        AnsiConsole.WriteLine("Writing identity addresses to a temp file...");

        var file = new StreamWriter(new FileStream(FilePaths.PATH_TO_IDENTITIES_FILE, FileMode.Create), Encoding.UTF8);

        await file.WriteLineAsync(a.IdentityData!.Address);
        await file.WriteLineAsync(b.IdentityData!.Address);

        await file.FlushAsync();
        file.Close();

        AnsiConsole.WriteLine(Emoji.Known.CheckMarkButton);
    }

    private static ApiResponse<T> DummyErrorResponse<T>()
    {
        return new ApiResponse<T>
        {
            ContentType = null,
            Error = new ApiError
            {
                Code = "error.dummy",
                Data = null,
                Message = "An error occured",
                Docs = "",
                Id = "",
                Time = DateTime.UtcNow
            },
            Status = HttpStatusCode.BadRequest
        };
    }
}
