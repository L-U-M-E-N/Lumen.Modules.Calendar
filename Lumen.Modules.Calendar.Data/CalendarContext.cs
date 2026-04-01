using Lumen.Modules.Calendar.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.Calendar.Data {
    public class CalendarContext : DbContext {
        public const string SCHEMA_NAME = "Calendar";

        public CalendarContext(DbContextOptions<CalendarContext> options) : base(options) {
        }

        public DbSet<CalendarPointInTime> Calendar { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var CalendarModelBuilder = modelBuilder.Entity<CalendarPointInTime>();
            CalendarModelBuilder.Property(x => x.Time)
                .HasColumnType("timestamp with time zone");

            CalendarModelBuilder.Property(x => x.Value)
                .HasColumnType("integer");

            CalendarModelBuilder.HasKey(x => x.Time);
        }
    }
}
