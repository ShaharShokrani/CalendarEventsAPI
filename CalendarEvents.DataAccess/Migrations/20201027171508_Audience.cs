using Microsoft.EntityFrameworkCore.Migrations;

namespace CalendarEvents.DataAccess.Migrations
{
    public partial class Audience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audience",
                table: "Events");
        }
    }
}
