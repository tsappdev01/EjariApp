using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIP.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSliderHomePage23100418454002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AppSliderHomePages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "AppSliderHomePages");
        }
    }
}
