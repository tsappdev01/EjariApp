using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class AddedMetaTitleEnMEtaDEscriptionEnMetaTitleArMetaDescriptionArToAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionAr",
                table: "AppAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "AppAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleAr",
                table: "AppAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "AppAmenities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescriptionAr",
                table: "AppAmenities");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "AppAmenities");

            migrationBuilder.DropColumn(
                name: "MetaTitleAr",
                table: "AppAmenities");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "AppAmenities");
        }
    }
}
