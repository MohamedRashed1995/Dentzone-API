using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class editcart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductPrice_ProductPriceId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductPriceId",
                table: "Carts");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductPriceId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductPriceId",
                table: "CartItems",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductPrice_ProductPriceId",
                table: "CartItems",
                column: "ProductPriceId",
                principalTable: "ProductPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductPrice_ProductPriceId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductPriceId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductPriceId",
                table: "Carts",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductPrice_ProductPriceId",
                table: "Carts",
                column: "ProductPriceId",
                principalTable: "ProductPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
