using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gambling_my_beloved.Migrations
{
    /// <inheritdoc />
    public partial class AddEffectedStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EffectedStocks",
                table: "StockEvents",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectedStocks",
                table: "StockEvents");
        }
    }
}
