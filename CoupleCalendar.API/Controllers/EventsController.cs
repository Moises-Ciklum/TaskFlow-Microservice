using CoupleCalendar.Application.DTOs;
using CoupleCalendar.Application.Services;
using CoupleCalendar.Core.Entities;
using CoupleCalendar.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoupleCalendar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repository; // No more Entity Framework here!
    private readonly IEventService _eventService;

    public EventsController(IEventRepository repository, IEventService eventService)
    {
        _repository = repository;
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto requestDto)
    {
        var newEvent = await _eventService.CreateEventAsync(requestDto);
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

        return Ok(response);
    }
}