using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("67a6f548-40ee-4d06-b93c-da10aa75d2aa"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"),
                column: "Name",
                value: "Inventory Manager");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("b8f8c440-7f59-4078-92fc-76ff2e8ed611"), new DateTime(2026, 2, 26, 8, 37, 1, 290, DateTimeKind.Utc).AddTicks(2862), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAEKkD0j0ugKhYWMemMA2fckjUDj+Hzq/wEqVhUEyA3sMRu0A3b1xVaZenudxigux1yg==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("d630ecea-d45c-4688-98d9-2faa34d0c6b1"), new Guid("8c2f4f3a-7f6d-4db8-8b02-4a04d31f35d6"), new Guid("b8f8c440-7f59-4078-92fc-76ff2e8ed611") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d630ecea-d45c-4688-98d9-2faa34d0c6b1"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("b8f8c440-7f59-4078-92fc-76ff2e8ed611"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1a5a84fb-23c3-4f9b-a122-4c5bc6c5cb2d"),
                column: "Name",
                value: "Inventory");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48"), new DateTime(2026, 2, 25, 11, 25, 7, 154, DateTimeKind.Utc).AddTicks(7451), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAECiX9z+o5JoeGEvhr2jnhSOc+Y0WA8XwKE8brihH0HVPPo5QxLVGYKpvqywQ2gt6+g==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("67a6f548-40ee-4d06-b93c-da10aa75d2aa"), new Guid("8c2f4f3a-7f6d-4db8-8b02-4a04d31f35d6"), new Guid("0f1acf3c-bef4-43e1-853e-56f5c9657b48") });
        }
    }
}
