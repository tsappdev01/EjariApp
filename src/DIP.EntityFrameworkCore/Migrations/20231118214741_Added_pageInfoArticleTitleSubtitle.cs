using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class AddedpageInfoArticleTitleSubtitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PageInfoArticleSubtitleAr",
                table: "AppPageInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageInfoArticleSubtitleEn",
                table: "AppPageInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageInfoArticleTilteAr",
                table: "AppPageInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageInfoArticleTilteEn",
                table: "AppPageInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageInfoArticleSubtitleAr",
                table: "AppPageInfos");

            migrationBuilder.DropColumn(
                name: "PageInfoArticleSubtitleEn",
                table: "AppPageInfos");

            migrationBuilder.DropColumn(
                name: "PageInfoArticleTilteAr",
                table: "AppPageInfos");

            migrationBuilder.DropColumn(
                name: "PageInfoArticleTilteEn",
                table: "AppPageInfos");
        }
    }
}
