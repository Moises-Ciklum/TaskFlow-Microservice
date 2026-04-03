using CoupleCalendar.Core.Entities;

namespace CoupleCalendar.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<CalendarEvent>> GetAllAsync();
        Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent);
    }
}
