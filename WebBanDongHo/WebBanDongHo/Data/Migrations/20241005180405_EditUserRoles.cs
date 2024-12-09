using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDongHo.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inventoryQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "discount",
                table: "OrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "users_id",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "timeActive",
                table: "AspNetUserRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_users_id",
                table: "Orders",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_users_id",
                table: "Orders",
                column: "users_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_users_id",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_users_id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "users_id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "timeActive",
                table: "AspNetUserRoles");

            migrationBuilder.AddColumn<int>(
                name: "inventoryQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
