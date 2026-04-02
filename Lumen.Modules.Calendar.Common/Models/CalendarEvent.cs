namespace Lumen.Modules.Calendar.Common.Models {
    public class CalendarEvent {
        public string Uid { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
