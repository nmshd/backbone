using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.SseServer.Versions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.SseServer.Controllers;

[V1]
[V2]
public class SseController : ControllerBase
{
    private readonly IEventQueue _eventQueue;
    private readonly IUserContext _userContext;
    private readonly IMediator _mediator;
    private readonly ILogger<SseController> _logger;

    public SseController(IEventQueue eventQueue, IUserContext userContext, IMediator mediator, ILogger<SseController> logger)
    {
        _eventQueue = eventQueue;
        _userContext = userContext;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/v{v:apiVersion}/sse")]
    [Authorize]
    public async Task Subscribe(CancellationToken cancellationToken)
    {
        var address = _userContext.GetAddress().Value;

        await _mediator.Send(new UpdateDeviceRegistrationCommand
        {
            Handle = "sse-handle", // this is just some dummy value; the SSE connector doesn't use it
            AppId = "sse-client", // this is just some dummy value; the SSE connector doesn't use it
            Platform = "sse"
        }, cancellationToken);

        Response.StatusCode = 200;
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        Response.Headers.ContentType = "text/event-stream";

        var streamWriter = new StreamWriter(Response.Body);

        try
        {
            _eventQueue.Register(address, cancellationToken);

            await streamWriter.SendServerSentEvent("ConnectionOpened");

            await foreach (var eventName in _eventQueue.DequeueFor(address, cancellationToken))
            {
                _logger.LogDebug("Sending event '{EventName}'...", eventName);
                await streamWriter.SendServerSentEvent(eventName);
                _logger.LogDebug("Event '{EventName}' successfully sent.", eventName);
            }
        }
        catch (ClientAlreadyRegisteredException)
        {
            // if it is already registered, everything is fine
        }
        catch (OperationCanceledException)
        {
            // this is expected when the client disconnects
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
        }
        finally
        {
            _eventQueue.Deregister(address);
            // we must NOT pass the cancellation token here, because otherwise the device registration would not be deleted in case the request was cancelled
            await _mediator.Send(new DeleteDeviceRegistrationCommand(), CancellationToken.None);
        }
    }
}

public static class StreamWriterExtensions
{
    public static async Task SendServerSentEvent(this StreamWriter streamWriter, string eventName)
    {
        await streamWriter.WriteLineAsync($"event: {eventName}");
        await streamWriter.WriteLineAsync("data: _");
        await streamWriter.WriteLineAsync();
        await streamWriter.FlushAsync();
    }
}
