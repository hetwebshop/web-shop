using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class FixDuplicatedUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTransactions_AspNetUsers_UserId1",
                table: "UserTransactions");

            migrationBuilder.DropIndex(
                name: "IX_UserTransactions_UserId1",
                table: "UserTransactions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserTransactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_UserId1",
                table: "UserTransactions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTransactions_AspNetUsers_UserId1",
                table: "UserTransactions",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
