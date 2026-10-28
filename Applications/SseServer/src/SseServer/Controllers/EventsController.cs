using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.SseServer.Controllers;

#pragma warning disable AV0014 // Preserve the existing model binding and error responses for SSE clients.
public class EventsController : ControllerBase
{
    private readonly IEventQueue _eventQueue;

    public EventsController(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }

    [AllowAnonymous]
    [HttpPost("/{address}/events")]
    public async Task<IActionResult> PostMessage([FromRoute] string address, [FromBody] CreateEventRequest request)
    {
        try
        {
            await _eventQueue.EnqueueFor(address, request.EventName, HttpContext.RequestAborted);
            return Ok();
        }
        catch (ClientNotFoundException)
        {
            return BadRequest(HttpError.ForProduction("error.platform.sseClientNotRegistered", $"An sse client for the address '{address}' is not registered.", ""));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
#pragma warning restore AV0014

public class CreateEventRequest
{
    public required string EventName { get; set; }
}
