using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteDeviceRegistration;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.SseServer.Controllers;

public class SseController : ControllerBase
{
    private readonly IEventQueue _eventQueue;
    private readonly IUserContext _userContext;
    private readonly ILogger<SseController> _logger;
    private readonly IServiceProvider _serviceProvider;

    // CATION: DO NOT INJECT ANYTHING THAT IS BAD TO KEEP ALIVE DURING THE ENTIRE LIFETIME OF THE SSE CONNECTION.
    // ALSO WATCH OUT FOR TRANSITIVE DEPENDENCIES.
    public SseController(IEventQueue eventQueue, IUserContext userContext, ILogger<SseController> logger, IServiceProvider serviceProvider)
    {
        _eventQueue = eventQueue;
        _userContext = userContext;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [HttpGet("/api/v1/sse")]
    [Authorize]
    public async Task Subscribe(CancellationToken cancellationToken)
    {
        var address = _userContext.GetAddress().Value;

        // We need a separate scope here, so that all DbContext instances are disposed after the MediatR command is executed. Otherwise, 
        // the DbContext would be kept alive until the SSE connection is closed.
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new UpdateDeviceRegistrationCommand
            {
                Handle = "sse-handle", // this is just some dummy value; the SSE connector doesn't use it
                AppId = "sse-client", // this is just some dummy value; the SSE connector doesn't use it
                Platform = "sse"
            }, cancellationToken);
        }

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
            await using var scope = _serviceProvider.CreateAsyncScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // we must NOT pass the cancellation token here, because otherwise the device registration would not be deleted in case the request was cancelled
            await mediator.Send(new DeleteDeviceRegistrationCommand(), CancellationToken.None);
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
