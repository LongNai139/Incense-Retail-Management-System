using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SV22T1080045.Shop.DataLayers.Migrations
{
    [DbContext(typeof(ShopDbContext))]
    [Migration("20261016120000_AddStockReimportLineReference")]
    public partial class AddStockReimportLineReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedOrderDetailId",
                table: "StockTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_RelatedOrderDetailId",
                table: "StockTransactions",
                column: "RelatedOrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_RelatedOrderDetailId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "RelatedOrderDetailId",
                table: "StockTransactions");
        }
    }
}
