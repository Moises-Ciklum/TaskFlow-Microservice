using CoupleCalendar.API.Hubs;
using CoupleCalendar.Application.DTOs;
using CoupleCalendar.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CoupleCalendar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IHubContext<CalendarHub> _hubContext;
    private readonly IEventService _eventService;

    public EventsController(IHubContext<CalendarHub> hubContext, IEventService eventService)
    {
        _hubContext = hubContext;
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto requestDto)
    {
        var newEvent = await _eventService.CreateEventAsync(requestDto);
        await _hubContext.Clients.All.SendAsync("ReceiveNewEvent", newEvent);
        return Ok(newEvent);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents([FromQuery] string? owner)
    {
        var response = await _eventService.GetAllEventsAsync(owner);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var isDeleted = await _eventService.DeleteEventAsync(id);

        if (!isDeleted)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveDeletedEvent", id);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDto requestDto)
    {
        var response = await _eventService.UpdateEventAsync(id, requestDto);

        if (response == null)
        {
            return NotFound();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveUpdatedEvent", response);

        return Ok(response);
    }
}