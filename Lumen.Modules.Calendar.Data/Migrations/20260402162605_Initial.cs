using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Modules.Calendar.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "calendar");

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                schema: "calendar",
                columns: table => new
                {
                    Uid = table.Column<string>(type: "text", nullable: false),
                    SourceLink = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => new { x.Uid, x.SourceLink });
                });

            migrationBuilder.CreateTable(
                name: "CalendarSources",
                schema: "calendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSources", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents",
                schema: "calendar");

            migrationBuilder.DropTable(
                name: "CalendarSources",
                schema: "calendar");
        }
    }
}
