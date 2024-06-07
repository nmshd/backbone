using Backbone.BuildingBlocks.API;
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
    private readonly IMediator _mediator;
    private readonly ILogger<SseController> _logger;

    public SseController(IEventQueue eventQueue, IUserContext userContext, IMediator mediator, ILogger<SseController> logger)
    {
        _eventQueue = eventQueue;
        _userContext = userContext;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("/api/v1/sse")]
    [Authorize]
    public async Task<IActionResult> Subscribe()
    {
        var address = _userContext.GetAddress().Value;

        await _mediator.Send(new UpdateDeviceRegistrationCommand
        {
            Handle = "sse-handle",
            AppId = "sse-client",
            Platform = "sse"
        });

        Response.StatusCode = 200;
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        Response.Headers.ContentType = "text/event-stream";

        var streamWriter = new StreamWriter(Response.Body);

        try
        {
            _eventQueue.Register(address);

            await foreach (var message in _eventQueue.DequeueFor(address, HttpContext.RequestAborted))
            {
                await streamWriter.WriteLineAsync($"data: {message}\n\n");
                await streamWriter.FlushAsync();
            }
        }
        catch (ClientAlreadyRegisteredException)
        {
            return BadRequest(HttpError.ForProduction("error.platform.sseClientAlreadyRegistered",
                $"An SSE client for your identity is already registered. You can only register once per identity.", ""));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
        }
        finally
        {
            _eventQueue.Deregister(address);
            await _mediator.Send(new DeleteDeviceRegistrationCommand());
        }

        return Ok();
    }
}
