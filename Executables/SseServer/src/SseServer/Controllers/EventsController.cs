using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.SseServer.Controllers;

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

public class CreateEventRequest
{
    public required string EventName { get; set; }
}
