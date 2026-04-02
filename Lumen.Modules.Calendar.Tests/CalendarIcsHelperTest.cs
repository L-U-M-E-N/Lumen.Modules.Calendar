using Lumen.Modules.Calendar.Common;
using Lumen.Modules.Calendar.Common.Models;

using Microsoft.Extensions.Configuration;

namespace Lumen.Modules.Calendar.Tests {
    public class CalendarIcsHelperTest {
        private readonly CalendarSource source;

        public CalendarIcsHelperTest() {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<CalendarIcsHelperTest>()
                .Build();

            source = new CalendarSource() {
                Id = Guid.NewGuid(),
                Name = "Test",
                Link = config["ICS_URL"]!,
            };
        }

        [Fact]
        public async Task GetEvents_ValidUrl_ReturnsEvents() {
            var from = DateTime.UtcNow.AddYears(-10);
            var to = DateTime.UtcNow.AddHours(-12);

            var events = await CalendarIcsHelper.GetEventsAsync(source, from, to);

            Assert.NotNull(events);
            Assert.NotEmpty(events);
        }

        [Fact]
        public async Task GetEvents_ValidUrl_AllEventsWithinWindow() {
            var from = DateTime.UtcNow.AddYears(-10);
            var to = DateTime.UtcNow.AddHours(-12);

            var events = await CalendarIcsHelper.GetEventsAsync(source, from, to);

            Assert.All(events, e => {
                Assert.True(e.Start >= from, $"Event \"{e.Title}\" starts before the window ({e.Start})");
                Assert.True(e.Start <= to, $"Event \"{e.Title}\" starts after the window ({e.Start})");
            });
        }

        [Fact]
        public async Task GetEvents_ValidUrl_EventsHaveRequiredFields() {
            var from = DateTime.UtcNow.AddYears(-10);
            var to = DateTime.UtcNow.AddHours(-12);

            var events = await CalendarIcsHelper.GetEventsAsync(source, from, to);

            Assert.All(events, e => {
                Assert.False(string.IsNullOrWhiteSpace(e.Uid), "Event is missing a UID");
                Assert.False(string.IsNullOrWhiteSpace(e.SourceName), "Event is missing SourceName");
                Assert.False(string.IsNullOrWhiteSpace(e.Title), "Event is missing a Title");
                Assert.True(e.End >= e.Start, "Event End is before Start");
            });
        }
    }
}
