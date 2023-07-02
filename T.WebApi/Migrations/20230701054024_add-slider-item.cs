using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addslideritem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SliderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SliderItem", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 7, 1, 5, 40, 23, 7, DateTimeKind.Utc).AddTicks(3951));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 7, 1, 5, 40, 23, 7, DateTimeKind.Utc).AddTicks(3956));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 7, 1, 5, 40, 23, 7, DateTimeKind.Utc).AddTicks(3957));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 7, 1, 5, 40, 23, 7, DateTimeKind.Utc).AddTicks(3958));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 7, 1, 5, 40, 23, 7, DateTimeKind.Utc).AddTicks(3959));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SliderItem");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3446));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3451));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3452));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3453));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 6, 29, 21, 37, 23, 284, DateTimeKind.Utc).AddTicks(3454));
        }
    }
}
