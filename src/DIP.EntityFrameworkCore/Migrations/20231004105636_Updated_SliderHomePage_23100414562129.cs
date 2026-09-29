using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSliderHomePage23100414562129 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "AppSliderHomePages",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AppSliderHomePages",
                newName: "TitleAr");

            migrationBuilder.RenameColumn(
                name: "ButtonTitle",
                table: "AppSliderHomePages",
                newName: "DescriptionEn");

            migrationBuilder.AddColumn<string>(
                name: "ButtonTitleAr",
                table: "AppSliderHomePages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButtonTitleEn",
                table: "AppSliderHomePages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "AppSliderHomePages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonTitleAr",
                table: "AppSliderHomePages");

            migrationBuilder.DropColumn(
                name: "ButtonTitleEn",
                table: "AppSliderHomePages");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "AppSliderHomePages");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "AppSliderHomePages",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TitleAr",
                table: "AppSliderHomePages",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "AppSliderHomePages",
                newName: "ButtonTitle");
        }
    }
}
