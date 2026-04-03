using CoupleCalendar.Core.Entities;

namespace CoupleCalendar.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<CalendarEvent>> GetAllAsync(string? owner = null);
        Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent);
        Task<bool> DeleteAsync(Guid id);
        Task<CalendarEvent?> UpdateAsync(Guid id, CalendarEvent calendarEvent);
    }
}
