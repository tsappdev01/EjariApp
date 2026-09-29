using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class updateLastEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "AppLastEventss",
                newName: "LocationEN");

            migrationBuilder.AddColumn<string>(
                name: "LocationAr",
                table: "AppLastEventss",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationAr",
                table: "AppLastEventss");

            migrationBuilder.RenameColumn(
                name: "LocationEN",
                table: "AppLastEventss",
                newName: "Location");
        }
    }
}
