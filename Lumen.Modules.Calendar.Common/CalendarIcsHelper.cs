using Lumen.Modules.Calendar.Common.Models;


namespace Lumen.Modules.Calendar.Common {
    public static class CalendarIcsHelper {
        private static readonly HttpClient _httpClient = new();

        public static async Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(CalendarSource source, DateTime from, DateTime to) {
            var icsContent = await _httpClient.GetStringAsync(source.Link);
            var calendar = Ical.Net.Calendar.Load(icsContent);

            if (calendar is null) {
                return [];
            }

            return [.. calendar.Events
                .Where((e) => e.DtStart is not null
                         && e.DtStart.AsUtc >= from
                         && e.DtStart.AsUtc <= to)
                .Select((e) => MapEvent(e, source.Name))];
        }

        private static CalendarEvent MapEvent(Ical.Net.CalendarComponents.CalendarEvent icsEvent, string sourceName) =>
            new() {
                Uid = icsEvent.Uid ?? string.Empty,
                SourceName = sourceName,
                Title = icsEvent.Summary ?? "No Title",
                Description = string.IsNullOrWhiteSpace(icsEvent.Description) ? null : icsEvent.Description.Trim(),
                Start = icsEvent.DtStart.AsUtc,
                End = icsEvent.DtEnd?.AsUtc ?? icsEvent.DtStart.AsUtc,
            };
    }
}
