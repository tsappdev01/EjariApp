using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class AddedMediaGalleryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MediaGalleryId",
                table: "AppMedias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppMediaGalleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummaryEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SummaryAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMediaGalleries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppMedias_MediaGalleryId",
                table: "AppMedias",
                column: "MediaGalleryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppMedias_AppMediaGalleries_MediaGalleryId",
                table: "AppMedias",
                column: "MediaGalleryId",
                principalTable: "AppMediaGalleries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppMedias_AppMediaGalleries_MediaGalleryId",
                table: "AppMedias");

            migrationBuilder.DropTable(
                name: "AppMediaGalleries");

            migrationBuilder.DropIndex(
                name: "IX_AppMedias_MediaGalleryId",
                table: "AppMedias");

            migrationBuilder.DropColumn(
                name: "MediaGalleryId",
                table: "AppMedias");
        }
    }
}
