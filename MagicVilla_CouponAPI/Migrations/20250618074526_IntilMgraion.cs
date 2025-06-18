using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class IntilMgraion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percent = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Created", "IsActive", "LastUpdated", "Name", "Percent" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc), true, new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc), "10% Discount", 10 },
                    { 2, new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc), true, new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc), "20% Discount", 20 },
                    { 3, new DateTime(2025, 6, 8, 7, 39, 44, 716, DateTimeKind.Utc), false, new DateTime(2025, 6, 13, 7, 39, 44, 716, DateTimeKind.Utc), "30% Discount", 30 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
