using System.Dynamic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class Handler : IRequestHandler<RegisterDeviceCommand, RegisterDeviceResponse>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(ChallengeValidator challengeValidator, IUserContext userContext, ILogger<Handler> logger, IIdentitiesRepository identitiesRepository)
    {
        _challengeValidator = challengeValidator;
        _userContext = userContext;
        _logger = logger;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<RegisterDeviceResponse> Handle(RegisterDeviceCommand command, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        if (await _identitiesRepository.HasBackupDevice(identity.Address, cancellationToken)) throw new ApplicationException(ApplicationErrors.Devices.BackupDeviceAlreadyExists());

        await _challengeValidator.Validate(command.SignedChallenge, PublicKey.FromBytes(identity.PublicKey));
        _logger.LogTrace("Successfully validated challenge.");

        var communicationLanguageResult = CommunicationLanguage.Create(command.CommunicationLanguage);

        var newDevice = identity.AddDevice(communicationLanguageResult.Value, _userContext.GetDeviceId());

        await _identitiesRepository.UpdateWithNewDevice(identity, command.DevicePassword);

        _logger.CreatedDevice();

        return new RegisterDeviceResponse(newDevice.User);
    }
}

public class DynamicJsonConverter : JsonConverter<dynamic>
{
    public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt64(out var i) ? i : reader.GetDouble(),
            JsonTokenType.String => reader.TryGetDateTime(out var datetime) ? datetime.ToString(CultureInfo.InvariantCulture) : reader.GetString()!,
            JsonTokenType.StartObject => ReadObject(JsonDocument.ParseValue(ref reader).RootElement),
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone() // use JsonElement as fallback.
        };
    }

    private object ReadObject(JsonElement jsonElement)
    {
        IDictionary<string, object?> expandoObject = new ExpandoObject();
        foreach (var obj in jsonElement.EnumerateObject())
        {
            var k = obj.Name;
            var value = ReadValue(obj.Value);
            expandoObject[k] = value;
        }

        return expandoObject;
    }

    private object? ReadValue(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Object => ReadObject(jsonElement),
            JsonValueKind.Array => ReadList(jsonElement),
            JsonValueKind.String => jsonElement.GetString()!,
            JsonValueKind.Number => jsonElement.TryGetInt64(out var i) ? i : jsonElement.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Undefined => null,
            JsonValueKind.Null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(jsonElement.ValueKind))
        };
    }

    private List<object?>? ReadList(JsonElement jsonElement)
    {
        var list = new List<object?>();
        jsonElement.EnumerateArray().ToList().ForEach(j => list.Add(ReadValue(j)));
        return list.Count == 0 ? null : list;
    }

    public override void Write(Utf8JsonWriter writer,
        object value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}

internal static partial class DeleteDeviceLogs
{
    [LoggerMessage(
        EventId = 219823,
        EventName = "Devices.RegisterDevice.RegisteredDevice",
        Level = LogLevel.Information,
        Message = "Successfully created device.")]
    public static partial void CreatedDevice(this ILogger logger);
}
