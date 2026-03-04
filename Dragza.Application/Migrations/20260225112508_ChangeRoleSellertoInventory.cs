using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleSellertoInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_User_SellerUserId",
                table: "ProductPrices");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f7fabf0f-483c-48be-89a1-0c726075b745"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryUserId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"), "Inventory" },
                    { new Guid("8c2f4f3a-7f6d-4db8-8b02-4a04d31f35d6"), "Admin" },
                    { new Guid("e48e5a9f-2074-4de9-a849-5c69fdd45e4e"), "User" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48"), new DateTime(2026, 2, 25, 11, 25, 7, 154, DateTimeKind.Utc).AddTicks(7451), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAECiX9z+o5JoeGEvhr2jnhSOc+Y0WA8XwKE8brihH0HVPPo5QxLVGYKpvqywQ2gt6+g==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("67a6f548-40ee-4d06-b93c-da10aa75d2aa"), new Guid("8c2f4f3a-7f6d-4db8-8b02-4a04d31f35d6"), new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48") });

            migrationBuilder.CreateIndex(
                name: "IX_Products_InventoryUserId",
                table: "Products",
                column: "InventoryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_User_InventoryUserId",
                table: "ProductPrices",
                column: "InventoryUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_User_InventoryUserId",
                table: "Products",
                column: "InventoryUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_User_InventoryUserId",
                table: "ProductPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_InventoryUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InventoryUserId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e48e5a9f-2074-4de9-a849-5c69fdd45e4e"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("67a6f548-40ee-4d06-b93c-da10aa75d2aa"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8c2f4f3a-7f6d-4db8-8b02-4a04d31f35d6"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48"));

            migrationBuilder.DropColumn(
                name: "InventoryUserId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "InventoryUserId",
                table: "ProductPrices",
                newName: "SellerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPrices_InventoryUserId",
                table: "ProductPrices",
                newName: "IX_ProductPrices_SellerUserId");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "User" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Seller" }
                });

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
    }
}
