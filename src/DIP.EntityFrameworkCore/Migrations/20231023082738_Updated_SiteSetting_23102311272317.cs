using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSiteSetting23102311272317 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosedDay1",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedDay2",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FridayWorkHours",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RamadanWorkDays",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RamadanWorkHours",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDays",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkHours",
                table: "AppSiteSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedDay1",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "ClosedDay2",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "FridayWorkHours",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "RamadanWorkDays",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "RamadanWorkHours",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "WorkDays",
                table: "AppSiteSettings");

            migrationBuilder.DropColumn(
                name: "WorkHours",
                table: "AppSiteSettings");
        }
    }
}
