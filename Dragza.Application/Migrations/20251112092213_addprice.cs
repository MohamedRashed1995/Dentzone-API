using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class addprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductPriceId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductPrice_ProductPriceId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductPriceId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "Carts");
        }
    }
}
