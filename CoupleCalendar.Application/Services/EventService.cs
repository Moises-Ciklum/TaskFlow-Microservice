using CoupleCalendar.Application.DTOs;
using CoupleCalendar.Core.Entities;
using CoupleCalendar.Core.Interfaces;

namespace CoupleCalendar.Application.Services;

public class EventService : IEventService
{
    // The Service talks to the Repository
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<CalendarEvent> CreateEventAsync(CreateEventDto requestDto)
    {
        // 1. Validate Business Rule: No overlapping events for the same owner
        var existingEvents = await _repository.GetAllAsync(requestDto.Owner);

        bool hasOverlap = existingEvents.Any(e =>
            requestDto.StartDate < e.EndDate &&
            requestDto.EndDate > e.StartDate);

        if (hasOverlap)
        {
            // If the rule is broken, we throw an exception that the Controller will catch
            throw new InvalidOperationException("El evento se solapa con otro turno existente.");
        }

        // 2. If valid, map to Entity
        var newEvent = new CalendarEvent
        {
            Title = requestDto.Title,
            StartDate = requestDto.StartDate,
            EndDate = requestDto.EndDate,
            Owner = requestDto.Owner,
            Type = requestDto.Type
        };

        // 3. Save using the Repository
        return await _repository.AddAsync(newEvent);
    }

    public async Task<IEnumerable<EventResponseDto>> GetAllEventsAsync(string? owner)
    {
        var eventsFromDb = await _repository.GetAllAsync(owner);

        return eventsFromDb.Select(e => new EventResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Owner = e.Owner,
            Type = e.Type.ToString()
        });
    }

    // NEW: Update mapping Entity <-> DTO
    public async Task<EventResponseDto?> UpdateEventAsync(Guid id, UpdateEventDto requestDto)
    {
        var existingEvents = await _repository.GetAllAsync(requestDto.Owner);
        var eventToUpdate = new CalendarEvent
        {
            Title = requestDto.Title,
            StartDate = requestDto.StartDate,
            EndDate = requestDto.EndDate,
            Owner = requestDto.Owner,
            Type = requestDto.Type
        };

        bool hasOverlap = existingEvents.Any(e =>
            e.Id != id &&
            eventToUpdate.StartDate < e.EndDate &&
            eventToUpdate.EndDate > e.StartDate);

        if (hasOverlap)
        {
            // If the rule is broken, we throw an exception that the Controller will catch
            throw new InvalidOperationException("El evento se solapa con otro turno existente.");
        }

        var updatedEvent = await _repository.UpdateAsync(id, eventToUpdate);

        if (updatedEvent == null) return null;

        return new EventResponseDto
        {
            Id = updatedEvent.Id,
            Title = updatedEvent.Title,
            StartDate = updatedEvent.StartDate,
            EndDate = updatedEvent.EndDate,
            Owner = updatedEvent.Owner,
            Type = updatedEvent.Type.ToString()
        };
    }

    // NEW: Delete
    public async Task<bool> DeleteEventAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}