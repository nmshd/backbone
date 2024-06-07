using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.SseServer.Controllers;

public class EventsController : ControllerBase
{
    private readonly IEventQueue _eventQueue;

    public EventsController(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }

    [HttpPost("/{address}/events")]
    public async Task<IActionResult> PostMessage([FromRoute] string address, [FromBody] CreateMessageRequest request)
    {
        try
        {
            await _eventQueue.EnqueueFor(address, request.Message, HttpContext.RequestAborted);
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

public class CreateMessageRequest
{
    public required string Message { get; set; }
}
