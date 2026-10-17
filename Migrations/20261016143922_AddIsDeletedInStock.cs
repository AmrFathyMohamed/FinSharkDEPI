using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinShark.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedInStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Stocks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CompanyName", "Industry", "IsActive", "IsDelete", "LastDiv", "MarketCap", "Purchase", "Symbol" },
                values: new object[,]
                {
                    { 1, "DEPI", "Tech", true, false, new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 250000000000m, 205.06m, "ABCD" },
                    { 2, "DECI", "Child", false, false, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 300000000000m, 505.06m, "EFGH" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Stocks");
        }
    }
}
