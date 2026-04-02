using Lumen.Modules.Calendar.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.Calendar.Data {
    public class CalendarContext(DbContextOptions<CalendarContext> options) : DbContext(options) {
        public const string SCHEMA_NAME = "calendar";

        public DbSet<CalendarSource> CalendarSources { get; set; } = null!;
        public DbSet<CalendarEvent> CalendarEvents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var calendarSourceBuilder = modelBuilder.Entity<CalendarSource>();
            calendarSourceBuilder.HasKey(x => x.Id);
            calendarSourceBuilder.Property(x => x.Id).ValueGeneratedOnAdd();

            var calendarEventBuilder = modelBuilder.Entity<CalendarEvent>();
            calendarEventBuilder.HasKey(x => new { x.Uid, x.SourceName });
            calendarEventBuilder.Property(x => x.Start)
                .HasColumnType("timestamp with time zone");
            calendarEventBuilder.Property(x => x.End)
                .HasColumnType("timestamp with time zone");
        }
    }
}
