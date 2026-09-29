using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class AddedMetaTitleEnMEtaDEscriptionEnMetaTitleArMetaDescriptionArToZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionAr",
                table: "AppZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "AppZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleAr",
                table: "AppZones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "AppZones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescriptionAr",
                table: "AppZones");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "AppZones");

            migrationBuilder.DropColumn(
                name: "MetaTitleAr",
                table: "AppZones");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "AppZones");
        }
    }
}
