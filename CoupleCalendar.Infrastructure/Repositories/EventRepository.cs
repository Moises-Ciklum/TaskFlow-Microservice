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

    public async Task<IEnumerable<CalendarEvent>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent)
    {
        _context.Events.Add(calendarEvent);
        await _context.SaveChangesAsync();
        return calendarEvent;
    }
}