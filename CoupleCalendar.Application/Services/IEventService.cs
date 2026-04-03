using CoupleCalendar.Application.DTOs;
using CoupleCalendar.Core.Entities;

namespace CoupleCalendar.Application.Services;

public interface IEventService
{
    Task<CalendarEvent> CreateEventAsync(CreateEventDto requestDto);
    Task<IEnumerable<EventResponseDto>> GetAllEventsAsync(string? owner);
    Task<EventResponseDto?> UpdateEventAsync(Guid id, UpdateEventDto requestDto);
    Task<bool> DeleteEventAsync(Guid id);
}