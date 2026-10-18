using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SV22T1080045.Shop.DataLayers.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPhoneIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "StockTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CustomerDebts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2026, 10, 8, 18, 37, 58, 288, DateTimeKind.Local).AddTicks(9350));

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Customers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone_IsDeleted",
                table: "Customers",
                columns: new[] { "Phone", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Phone",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Phone_IsDeleted",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CustomerDebts");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2026, 9, 23, 16, 42, 6, 762, DateTimeKind.Local).AddTicks(670));
        }
    }
}
