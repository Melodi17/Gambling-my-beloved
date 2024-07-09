using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gambling_my_beloved.Migrations
{
    /// <inheritdoc />
    public partial class AddVolatility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Volatility",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volatility",
                table: "Stocks");
        }
    }
}
