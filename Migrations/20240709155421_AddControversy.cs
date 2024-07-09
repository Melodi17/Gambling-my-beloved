using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gambling_my_beloved.Migrations
{
    /// <inheritdoc />
    public partial class AddControversy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Controversy",
                table: "Companies",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Controversy",
                table: "Companies");
        }
    }
}
