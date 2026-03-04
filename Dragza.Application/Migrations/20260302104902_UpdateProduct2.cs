using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProduct2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_InventoryUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InventoryUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InventoryUserId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_InventoryUserId",
                table: "Products",
                column: "InventoryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_User_InventoryUserId",
                table: "Products",
                column: "InventoryUserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
