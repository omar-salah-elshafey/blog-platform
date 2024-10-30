using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class seedingRolesAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4960480c-20db-4901-b385-954cb665b54f", "3", "Reader", "READER" },
                    { "7cce8429-2590-4157-b950-42b912d66f7e", "2", "Author", "AUTHOR" },
                    { "e2955fae-bc9a-4ed0-82f3-7a54cd7a2aa7", "1", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "11f6dbf7-e2fd-4add-acf4-f4d3df7d7aba", 0, "19a85f94-ffb2-44da-8be1-494f2a0bc121", "omarsalah@test.com", true, "Omar", "Salah", false, null, "OMARSALAH@TEST.COM", "OMAR_SALAH", "AQAAAAIAAYagAAAAECZJvz91Si6xsI30yIRpQkM0N7QDfVatIy/LM0asp5Aj7/P/BOrpUbxE8nHr/xKdzQ==", null, false, "7addac2f-f701-4f06-8721-3313027bb478", false, "omar_salah" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4960480c-20db-4901-b385-954cb665b54f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7cce8429-2590-4157-b950-42b912d66f7e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2955fae-bc9a-4ed0-82f3-7a54cd7a2aa7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11f6dbf7-e2fd-4add-acf4-f4d3df7d7aba");
        }
    }
}
