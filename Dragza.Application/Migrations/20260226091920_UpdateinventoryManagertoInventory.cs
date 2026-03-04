using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateinventoryManagertoInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"),
                column: "Name",
                value: "Inventory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"),
                column: "Name",
                value: "Inventory Manager");
        }
    }
}
