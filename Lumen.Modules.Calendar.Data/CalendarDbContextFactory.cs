using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lumen.Modules.Calendar.Data {
    public class CalendarDbContextFactory : IDesignTimeDbContextFactory<CalendarContext> {
        public CalendarContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<CalendarContext>();
            optionsBuilder.UseNpgsql();

            return new CalendarContext(optionsBuilder.Options);
        }
    }
}
