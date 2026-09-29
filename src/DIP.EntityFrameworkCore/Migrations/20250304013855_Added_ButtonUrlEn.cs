using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class Added_ButtonUrlEn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ButtonUrl",
                table: "AppSliderHomePages",
                newName: "ButtonUrlEn");

            migrationBuilder.AddColumn<string>(
                name: "ButtonUrlAr",
                table: "AppSliderHomePages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonUrlAr",
                table: "AppSliderHomePages");

            migrationBuilder.RenameColumn(
                name: "ButtonUrlEn",
                table: "AppSliderHomePages",
                newName: "ButtonUrl");
        }
    }
}
