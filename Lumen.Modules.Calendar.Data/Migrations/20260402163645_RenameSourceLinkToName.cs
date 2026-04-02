using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Modules.Calendar.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameSourceLinkToName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SourceLink",
                schema: "calendar",
                table: "CalendarEvents",
                newName: "SourceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SourceName",
                schema: "calendar",
                table: "CalendarEvents",
                newName: "SourceLink");
        }
    }
}
