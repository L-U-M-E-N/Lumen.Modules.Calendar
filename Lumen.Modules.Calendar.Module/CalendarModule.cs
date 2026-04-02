using Lumen.Modules.Calendar.Common;
using Lumen.Modules.Calendar.Data;
using Lumen.Modules.Sdk;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.Calendar.Module {
    public class CalendarModule(IEnumerable<ConfigEntry> configEntries, ILogger<LumenModuleBase> logger, IServiceProvider provider) : LumenModuleBase(configEntries, logger, provider) {

        public override Task InitAsync(LumenModuleRunsOnFlag currentEnv) {
            return RunAsync(currentEnv, DateTime.UtcNow);
        }

        public override async Task RunAsync(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            try {
                logger.LogTrace($"[{nameof(CalendarModule)}] Running tasks ...");

                switch (currentEnv) {
                    case LumenModuleRunsOnFlag.API:
                        await RunAPIAsync(date);
                        break;
                    case LumenModuleRunsOnFlag.UI:
                        await RunUIAsync(date);
                        break;
                }

                logger.LogTrace($"[{nameof(CalendarModule)}] Running tasks ... Done!");
            } catch (Exception ex) {
                logger.LogError(ex, $"[{nameof(CalendarModule)}] Error when running tasks.");
            }
        }

        private async Task RunAPIAsync(DateTime date) {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CalendarContext>();

            var from = date.AddMonths(-1);
            var to = date.AddHours(-12);

            var sources = await context.CalendarSources.ToListAsync();

            if (sources.Count == 0) {
                logger.LogWarning($"[{nameof(CalendarModule)}] No calendar sources found in database, skipping.");
                return;
            }

            foreach (var source in sources) {
                logger.LogInformation($"[{nameof(CalendarModule)}] Syncing source \"{source.Name}\" ({source.Link}) ...");

                try {
                    var events = await CalendarIcsHelper.GetEventsAsync(source, from, to);

                    foreach (var calEvent in events) {
                        var existing = await context.CalendarEvents
                            .FirstOrDefaultAsync(x => x.Uid == calEvent.Uid && x.SourceName == calEvent.SourceName);

                        if (existing is null) {
                            logger.LogTrace($"[{nameof(CalendarModule)}] Inserting event \"{calEvent.Title}\" (uid={calEvent.Uid})");
                            context.CalendarEvents.Add(calEvent);
                        } else {
                            logger.LogTrace($"[{nameof(CalendarModule)}] Updating event \"{calEvent.Title}\" (uid={calEvent.Uid})");
                            existing.Title = calEvent.Title;
                            existing.Description = calEvent.Description;
                            existing.Start = calEvent.Start;
                            existing.End = calEvent.End;
                            context.CalendarEvents.Update(existing);
                        }
                    }

                    logger.LogInformation($"[{nameof(CalendarModule)}] Synced {events.Count} event(s) from \"{source.Name}\".");
                } catch (Exception ex) {
                    logger.LogError(ex, $"[{nameof(CalendarModule)}] Failed to sync source \"{source.Name}\" ({source.Link}).");
                }
            }

            await context.SaveChangesAsync();
        }

        private Task RunUIAsync(DateTime date) {
            // TODO
            return Task.CompletedTask;
        }

        public override bool ShouldRunNow(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            return currentEnv switch {
                LumenModuleRunsOnFlag.API => date.Second == 0 && date.Minute == 0 && date.Hour % 6 == 0,
                LumenModuleRunsOnFlag.UI => false,
                _ => false,
            };
        }

        public override Task ShutdownAsync() {
            return Task.CompletedTask;
        }

        public static new void SetupServices(LumenModuleRunsOnFlag currentEnv, IServiceCollection serviceCollection, string? postgresConnectionString) {
            if (currentEnv == LumenModuleRunsOnFlag.API) {
                serviceCollection.AddDbContext<CalendarContext>(o =>
                    o.UseNpgsql(postgresConnectionString,
                        x => x.MigrationsHistoryTable("__EFMigrationsHistory", CalendarContext.SCHEMA_NAME)));
            }
        }

        public override Type GetDatabaseContextType() {
            return typeof(CalendarContext);
        }
    }
}
