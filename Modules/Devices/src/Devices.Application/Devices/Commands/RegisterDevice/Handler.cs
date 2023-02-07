using System.Dynamic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class Handler : IRequestHandler<RegisterDeviceCommand, RegisterDeviceResponse>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly IDevicesDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public Handler(IDevicesDbContext dbContext, UserManager<ApplicationUser> userManager, ChallengeValidator challengeValidator, IUserContext userContext, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _challengeValidator = challengeValidator;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<RegisterDeviceResponse> Handle(RegisterDeviceCommand command, CancellationToken cancellationToken)
    {
        var identity = await _dbContext
            .Set<Identity>()
            .Include(i => i.Devices)
            .FirstWithAddress(_userContext.GetAddress(), cancellationToken);

        await _challengeValidator.Validate(command.SignedChallenge, PublicKey.FromBytes(identity.PublicKey));

        _logger.LogTrace("Successfully validated challenge.");

        var user = new ApplicationUser(identity, _userContext.GetDeviceId());

        var createUserResult = await _userManager.CreateAsync(user, command.DevicePassword);

        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));

        _logger.LogTrace($"Successfully created device. Device ID: {user.DeviceId}, User ID: {user.Id}, Username: {user.UserName}");

        return new RegisterDeviceResponse
        {
            Id = user.DeviceId,
            Username = user.UserName,
            CreatedByDevice = user.Device.CreatedByDevice,
            CreatedAt = user.Device.CreatedAt
        };
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
            JsonTokenType.String => reader.TryGetDateTime(out var datetime) ? datetime.ToString(CultureInfo.InvariantCulture) : reader.GetString(),
            JsonTokenType.StartObject => ReadObject(JsonDocument.ParseValue(ref reader).RootElement),
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone() // use JsonElement as fallback.
        };
    }

    private object ReadObject(JsonElement jsonElement)
    {
        IDictionary<string, object> expandoObject = new ExpandoObject();
        foreach (var obj in jsonElement.EnumerateObject())
        {
            var k = obj.Name;
            var value = ReadValue(obj.Value);
            expandoObject[k] = value;
        }

        return expandoObject;
    }

    private object ReadValue(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.Object => ReadObject(jsonElement),
            JsonValueKind.Array => ReadList(jsonElement),
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.TryGetInt64(out var i) ? i : jsonElement.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Undefined => null,
            JsonValueKind.Null => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private object ReadList(JsonElement jsonElement)
    {
        var list = new List<object>();
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
