using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SV22T1080045.Shop.DataLayers.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchemaForAuthOtpCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Customers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Customers",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Customers",
                newName: "CustomerName");

            migrationBuilder.CreateTable(
                name: "GuestOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PhoneHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PhoneLastFour = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ConvertedCustomerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_GuestOrders_Customers_ConvertedCustomerId",
                        column: x => x.ConvertedCustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GuestOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneOtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    FailCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneOtps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxUsage = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2026, 4, 16, 12, 39, 57, 956, DateTimeKind.Local).AddTicks(4784));

            migrationBuilder.CreateIndex(
                name: "IX_GuestOrders_ConvertedCustomerId",
                table: "GuestOrders",
                column: "ConvertedCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestOrders_PhoneHash",
                table: "GuestOrders",
                column: "PhoneHash");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneOtps_Phone_Purpose",
                table: "PhoneOtps",
                columns: new[] { "Phone", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Code",
                table: "Vouchers",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestOrders");

            migrationBuilder.DropTable(
                name: "PhoneOtps");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Customers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Customers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Customers",
                newName: "FullName");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2026, 1, 9, 19, 13, 21, 406, DateTimeKind.Local).AddTicks(4480));
        }
    }
}
