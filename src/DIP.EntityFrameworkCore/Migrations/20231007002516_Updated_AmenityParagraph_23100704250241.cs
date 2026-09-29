using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAmenityParagraph23100704250241 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AmenityId",
                table: "AppAmenityParagraphs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppAmenityParagraphs_AmenityId",
                table: "AppAmenityParagraphs",
                column: "AmenityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAmenityParagraphs_AppAmenities_AmenityId",
                table: "AppAmenityParagraphs",
                column: "AmenityId",
                principalTable: "AppAmenities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAmenityParagraphs_AppAmenities_AmenityId",
                table: "AppAmenityParagraphs");

            migrationBuilder.DropIndex(
                name: "IX_AppAmenityParagraphs_AmenityId",
                table: "AppAmenityParagraphs");

            migrationBuilder.DropColumn(
                name: "AmenityId",
                table: "AppAmenityParagraphs");
        }
    }
}
