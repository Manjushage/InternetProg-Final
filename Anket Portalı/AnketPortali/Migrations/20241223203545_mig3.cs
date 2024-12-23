using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnketPortali.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2becc45-aa0c-4d51-b54c-0533c50a19a7");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "dca9e1ef-adaf-425c-a314-242a2bcd561b", "7eb95f71-8289-42c1-b0c5-5e55caeaa764" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dca9e1ef-adaf-425c-a314-242a2bcd561b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7eb95f71-8289-42c1-b0c5-5e55caeaa764");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2b09ab06-413b-43b0-86d6-55b79b867691", null, "Admin", "ADMIN" },
                    { "f3c6d83f-add9-4a3f-b3b9-73719c4f3e42", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e37a1e76-b82d-4b32-a091-a37d1785711e", 0, "4638f35b-9cb5-4da5-a779-16d9ac496005", "admin@admin.com", true, null, null, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAEMsRpFMWXSQgVzUAhQhVPvlxHh/0RHBAe+Iu8yoKiYh1eVV4vav9qkMKk/7tLRD7Qg==", null, false, "fcd3cad6-741f-4d6b-89b4-28b40f05446b", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "2b09ab06-413b-43b0-86d6-55b79b867691", "e37a1e76-b82d-4b32-a091-a37d1785711e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3c6d83f-add9-4a3f-b3b9-73719c4f3e42");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2b09ab06-413b-43b0-86d6-55b79b867691", "e37a1e76-b82d-4b32-a091-a37d1785711e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b09ab06-413b-43b0-86d6-55b79b867691");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e37a1e76-b82d-4b32-a091-a37d1785711e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a2becc45-aa0c-4d51-b54c-0533c50a19a7", null, "User", "USER" },
                    { "dca9e1ef-adaf-425c-a314-242a2bcd561b", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7eb95f71-8289-42c1-b0c5-5e55caeaa764", 0, "ee68e503-95b8-49a1-a931-00a3657a0b56", "admin@example.com", true, null, null, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEFpAKl715bvVGRBzKQKfTIHGURKJNwb+j8moSOTydgsH9MoUhxXGl/tFG6f9UsE6zg==", null, false, "8e77e9e5-b5cc-49c6-82c9-6185ff4e6f36", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "dca9e1ef-adaf-425c-a314-242a2bcd561b", "7eb95f71-8289-42c1-b0c5-5e55caeaa764" });
        }
    }
}
