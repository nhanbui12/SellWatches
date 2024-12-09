using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDongHo.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTotalForOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "total",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total",
                table: "OrderDetails");
        }
    }
}
