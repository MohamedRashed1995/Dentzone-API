using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("15b94d41-a161-44b1-9a6f-baad311fcef5"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb"));

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MainCategoryId",
                table: "ProductPrices");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "User",
                newName: "FullName");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "Password", "PhoneNumber" },
                values: new object[] { new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1"), new DateTime(2026, 2, 22, 11, 4, 51, 351, DateTimeKind.Utc).AddTicks(9357), "admin@dentzone.com", "Admin User", true, false, "AQAAAAIAAYagAAAAEA7bLqm/rqdEptBRps/E4oRThjt4ns9hXRE8xPoEto8rHdyBJLu2OETMlCLu9732BQ==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("538e5795-f68b-4db4-98a9-5024b8dc407c"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("538e5795-f68b-4db4-98a9-5024b8dc407c"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("7a85d710-9391-4a76-b1a5-8415a9e4efa1"));

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "User",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "MainCategoryId",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsDeleted", "LastName", "Password", "PhoneNumber" },
                values: new object[] { new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb"), new DateTime(2026, 2, 22, 8, 48, 14, 784, DateTimeKind.Utc).AddTicks(4434), "admin@dentzone.com", "Admin", true, false, "User", "AQAAAAIAAYagAAAAEFHMq2S70CaDAtJqK4i2PqLDEeQ1nlndW4HYZNKQ5DdYC3sv3XvlYtTKgfghZnQt8g==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("15b94d41-a161-44b1-9a6f-baad311fcef5"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb") });
        }
    }
}
