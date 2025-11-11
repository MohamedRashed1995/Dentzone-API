using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class addInvintoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryUserId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_InventoryUserId",
                table: "CartItems",
                column: "InventoryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_User_InventoryUserId",
                table: "CartItems",
                column: "InventoryUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_User_InventoryUserId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_InventoryUserId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "InventoryUserId",
                table: "CartItems");
        }
    }
}
