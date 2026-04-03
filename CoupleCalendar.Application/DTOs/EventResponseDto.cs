namespace CoupleCalendar.Application.DTOs
{
    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Owner { get; set; } = string.Empty;

        // Notice this is a string, not the Enum! We want the frontend to read "Work", not "0".
        public string Type { get; set; } = string.Empty;
    }
}
