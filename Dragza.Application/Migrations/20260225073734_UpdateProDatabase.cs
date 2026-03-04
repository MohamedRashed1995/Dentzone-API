using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_User_InventoryUserId",
                table: "ProductPrices");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("538e5795-f68b-4db4-98a9-5024b8dc407c"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1"));

            migrationBuilder.RenameColumn(
                name: "InventoryUserId",
                table: "ProductPrices",
                newName: "SellerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPrices_InventoryUserId",
                table: "ProductPrices",
                newName: "IX_ProductPrices_SellerUserId");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("26e86849-8112-424e-8a47-d5aa8ffa5521"), new DateTime(2026, 2, 25, 7, 37, 33, 51, DateTimeKind.Utc).AddTicks(9136), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAEBbiYX9JsSzBVIoFiuGb1gy5DD26DTDr9OLBEukCJgePkonOISTuRx/0LIoKwSrqVw==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("f7fabf0f-483c-48be-89a1-0c726075b745"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("26e86849-8112-424e-8a47-d5aa8ffa5521") });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_User_SellerUserId",
                table: "ProductPrices",
                column: "SellerUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_User_SellerUserId",
                table: "ProductPrices");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f7fabf0f-483c-48be-89a1-0c726075b745"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("26e86849-8112-424e-8a47-d5aa8ffa5521"));

            migrationBuilder.RenameColumn(
                name: "SellerUserId",
                table: "ProductPrices",
                newName: "InventoryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPrices_SellerUserId",
                table: "ProductPrices",
                newName: "IX_ProductPrices_InventoryUserId");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1"), new DateTime(2026, 2, 22, 11, 4, 51, 351, DateTimeKind.Utc).AddTicks(9357), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAEA7bLqm/rqdEptBRps/E4oRThjt4ns9hXRE8xPoEto8rHdyBJLu2OETMlCLu9732BQ==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("538e5795-f68b-4db4-98a9-5024b8dc407c"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1") });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_User_InventoryUserId",
                table: "ProductPrices",
                column: "InventoryUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
