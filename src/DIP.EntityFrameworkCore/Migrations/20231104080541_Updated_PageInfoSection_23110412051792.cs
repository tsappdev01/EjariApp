using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPageInfoSection23110412051792 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SummaryAr",
                table: "AppPageInfoSections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SummaryEn",
                table: "AppPageInfoSections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummaryAr",
                table: "AppPageInfoSections");

            migrationBuilder.DropColumn(
                name: "SummaryEn",
                table: "AppPageInfoSections");
        }
    }
}
