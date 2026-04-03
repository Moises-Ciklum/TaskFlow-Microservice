using CoupleCalendar.Core.Entities;

namespace CoupleCalendar.Application.DTOs
{
    public class CreateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Owner { get; set; } = string.Empty;
        public EventType Type { get; set; }
    }
}
