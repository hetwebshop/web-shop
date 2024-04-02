using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RenameUserEducations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEducation_AspNetUsers_UserId",
                table: "UserEducation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEducation",
                table: "UserEducation");

            migrationBuilder.RenameTable(
                name: "UserEducation",
                newName: "UserEducations");

            migrationBuilder.RenameIndex(
                name: "IX_UserEducation_UserId",
                table: "UserEducations",
                newName: "IX_UserEducations_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEducations",
                table: "UserEducations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEducations_AspNetUsers_UserId",
                table: "UserEducations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEducations_AspNetUsers_UserId",
                table: "UserEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEducations",
                table: "UserEducations");

            migrationBuilder.RenameTable(
                name: "UserEducations",
                newName: "UserEducation");

            migrationBuilder.RenameIndex(
                name: "IX_UserEducations_UserId",
                table: "UserEducation",
                newName: "IX_UserEducation_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEducation",
                table: "UserEducation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEducation_AspNetUsers_UserId",
                table: "UserEducation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
