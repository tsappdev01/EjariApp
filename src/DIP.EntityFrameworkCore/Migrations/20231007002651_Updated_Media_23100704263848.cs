using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMedia23100704263848 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AmenityParagraphId",
                table: "AppMedias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMedias_AmenityParagraphId",
                table: "AppMedias",
                column: "AmenityParagraphId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppMedias_AppAmenityParagraphs_AmenityParagraphId",
                table: "AppMedias",
                column: "AmenityParagraphId",
                principalTable: "AppAmenityParagraphs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppMedias_AppAmenityParagraphs_AmenityParagraphId",
                table: "AppMedias");

            migrationBuilder.DropIndex(
                name: "IX_AppMedias_AmenityParagraphId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "AmenityParagraphId",
                table: "AppMedias");
        }
    }
}
