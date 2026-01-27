using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class uniqproductCOde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCode",
                table: "Product",
                column: "ProductCode",
                unique: true,
                filter: "[ProductCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_ProductCode",
                table: "Product");
        }
    }
}
