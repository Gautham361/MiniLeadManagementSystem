using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Leads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password", "Username" },
                values: new object[] { new DateTime(2026, 4, 12, 2, 48, 53, 94, DateTimeKind.Utc).AddTicks(9595), "Gowtham@123", "Gowtham" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Leads",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password", "Username" },
                values: new object[] { new DateTime(2026, 4, 10, 23, 25, 18, 771, DateTimeKind.Utc).AddTicks(6525), "Admin@123", "admin" });
        }
    }
}
