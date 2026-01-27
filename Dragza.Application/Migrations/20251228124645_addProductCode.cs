using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class addProductCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCode",
                table: "Product",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Product");
        }
    }
}
