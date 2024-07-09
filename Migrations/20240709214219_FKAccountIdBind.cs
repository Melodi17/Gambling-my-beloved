using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gambling_my_beloved.Migrations
{
    /// <inheritdoc />
    public partial class FKAccountIdBind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockOwnerships_AspNetUsers_AccountId1",
                table: "StockOwnerships");

            migrationBuilder.DropIndex(
                name: "IX_StockOwnerships_AccountId1",
                table: "StockOwnerships");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "StockOwnerships");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "StockOwnerships",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_StockOwnerships_AccountId",
                table: "StockOwnerships",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockOwnerships_AspNetUsers_AccountId",
                table: "StockOwnerships",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockOwnerships_AspNetUsers_AccountId",
                table: "StockOwnerships");

            migrationBuilder.DropIndex(
                name: "IX_StockOwnerships_AccountId",
                table: "StockOwnerships");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "StockOwnerships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountId1",
                table: "StockOwnerships",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockOwnerships_AccountId1",
                table: "StockOwnerships",
                column: "AccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StockOwnerships_AspNetUsers_AccountId1",
                table: "StockOwnerships",
                column: "AccountId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
