using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDongHo.Data.Migrations
{
    /// <inheritdoc />
    public partial class addimages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "images",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "image1",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image2",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image3",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "image2",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "image3",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "images",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
