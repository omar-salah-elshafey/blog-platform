using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class assigningAdminToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e2955fae-bc9a-4ed0-82f3-7a54cd7a2aa7", "11f6dbf7-e2fd-4add-acf4-f4d3df7d7aba" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e2955fae-bc9a-4ed0-82f3-7a54cd7a2aa7", "11f6dbf7-e2fd-4add-acf4-f4d3df7d7aba" });

        }
    }
}
