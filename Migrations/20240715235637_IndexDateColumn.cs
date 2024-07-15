using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gambling_my_beloved.Migrations
{
    /// <inheritdoc />
    public partial class IndexDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StockEvents_Date",
                table: "StockEvents",
                column: "Date",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_PricePeriod_Date",
                table: "PricePeriod",
                column: "Date",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockEvents_Date",
                table: "StockEvents");

            migrationBuilder.DropIndex(
                name: "IX_PricePeriod_Date",
                table: "PricePeriod");
        }
    }
}
