using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dragza.Application.Migrations
{
    /// <inheritdoc />
    public partial class kkasdddddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f29f9a11-f04b-4d6b-939a-150602331b9d"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsDeleted", "LastName", "Password", "PhoneNumber" },
                values: new object[] { new Guid("2329f6e6-f539-4656-bbc7-d6becf71049f"), new DateTime(2026, 2, 18, 23, 52, 10, 929, DateTimeKind.Utc).AddTicks(3769), "admin@dentzone.com", "Admin", true, false, "User", "AQAAAAIAAYagAAAAEJQS9kwC8X5yb77KZaF0piRJtlCY3aY+owBTyXO5s6DoLIAY4vOfbVVN60KYp5Ai5A==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("e1871fab-3e21-4b74-8982-5ee99e33cab1"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("2329f6e6-f539-4656-bbc7-d6becf71049f") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 2, 18, 23, 29, 5, 501, DateTimeKind.Utc).AddTicks(900), "admin@dentzone.com", "Admin", true, false, "User", "AQAAAAIAAYagAAAAEAA76BfKIbSURcBQmN1/Ed0K7fyg3EN7W5K4YEVm/WQ+W5xipyk62ckM6c+dHN46uA==", "01234567890" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { new Guid("f29f9a11-f04b-4d6b-939a-150602331b9d"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000000") });
        }
    }
}
