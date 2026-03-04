using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class MonsterServerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("e1871fab-3e21-4b74-8982-5ee99e33cab1"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2329f6e6-f539-4656-bbc7-d6becf71049f"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsDeleted", "LastName", "Password", "PhoneNumber" },
                values: new object[] { new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb"), new DateTime(2026, 2, 22, 8, 48, 14, 784, DateTimeKind.Utc).AddTicks(4434), "admin@dentzone.com", "Admin", true, false, "User", "AQAAAAIAAYagAAAAEFHMq2S70CaDAtJqK4i2PqLDEeQ1nlndW4HYZNKQ5DdYC3sv3XvlYtTKgfghZnQt8g==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("15b94d41-a161-44b1-9a6f-baad311fcef5"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("15b94d41-a161-44b1-9a6f-baad311fcef5"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("bb715345-31dd-44d3-84a8-2b55eba3c3bb"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsDeleted", "LastName", "Password", "PhoneNumber" },
                values: new object[] { new Guid("2329f6e6-f539-4656-bbc7-d6becf71049f"), new DateTime(2026, 2, 18, 23, 52, 10, 929, DateTimeKind.Utc).AddTicks(3769), "admin@dentzone.com", "Admin", true, false, "User", "AQAAAAIAAYagAAAAEJQS9kwC8X5yb77KZaF0piRJtlCY3aY+owBTyXO5s6DoLIAY4vOfbVVN60KYp5Ai5A==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("e1871fab-3e21-4b74-8982-5ee99e33cab1"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("2329f6e6-f539-4656-bbc7-d6becf71049f") });
        }
    }
}
