using CoupleCalendar.Core.Entities;
using CoupleCalendar.Core.Interfaces;
using CoupleCalendar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoupleCalendar.Infrastructure.Repositories;

// We inherit from the interface to guarantee we fulfill the contract
public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CalendarEvent>> GetAllAsync(string? owner = null)
    {
        var query = _context.Events.AsQueryable();

        if (!string.IsNullOrEmpty(owner))
        {
            query = query.Where(e => e.Owner == owner);
        }

        return await query.ToListAsync();
    }

    public async Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent)
    {
        _context.Events.Add(calendarEvent);
        await _context.SaveChangesAsync();
        return calendarEvent;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var eventToDelete = await _context.Events.FindAsync(id);
        if (eventToDelete == null)
        {
            return false;
        }
        _context.Events.Remove(eventToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CalendarEvent?> UpdateAsync(Guid id, CalendarEvent calendarEvent)
    {
        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null)
        {
            return null;
        }
        existingEvent.Title = calendarEvent.Title;
        existingEvent.StartDate = calendarEvent.StartDate;
        existingEvent.EndDate = calendarEvent.EndDate;
        existingEvent.Owner = calendarEvent.Owner;
        existingEvent.Type = calendarEvent.Type;

        await _context.SaveChangesAsync();
        return existingEvent;
    }
}