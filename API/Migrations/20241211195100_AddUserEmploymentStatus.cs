using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddUserEmploymentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmploymentStatusId",
                table: "UserJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentStatusId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmploymentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_EmploymentStatusId",
                table: "UserJobPosts",
                column: "EmploymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmploymentStatusId",
                table: "AspNetUsers",
                column: "EmploymentStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmploymentStatuses_EmploymentStatusId",
                table: "AspNetUsers",
                column: "EmploymentStatusId",
                principalTable: "EmploymentStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_EmploymentStatuses_EmploymentStatusId",
                table: "UserJobPosts",
                column: "EmploymentStatusId",
                principalTable: "EmploymentStatuses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmploymentStatuses_EmploymentStatusId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_EmploymentStatuses_EmploymentStatusId",
                table: "UserJobPosts");

            migrationBuilder.DropTable(
                name: "EmploymentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_EmploymentStatusId",
                table: "UserJobPosts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmploymentStatusId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmploymentStatusId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "EmploymentStatusId",
                table: "AspNetUsers");
        }
    }
}
