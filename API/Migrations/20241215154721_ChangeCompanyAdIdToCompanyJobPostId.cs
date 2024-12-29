using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class ChangeCompanyAdIdToCompanyJobPostId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_CompanyJobPosts_CompanyJobPostId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "CompanyAdId",
                table: "UserApplications");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyJobPostId",
                table: "UserApplications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_CompanyJobPosts_CompanyJobPostId",
                table: "UserApplications",
                column: "CompanyJobPostId",
                principalTable: "CompanyJobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_CompanyJobPosts_CompanyJobPostId",
                table: "UserApplications");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyJobPostId",
                table: "UserApplications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompanyAdId",
                table: "UserApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_CompanyJobPosts_CompanyJobPostId",
                table: "UserApplications",
                column: "CompanyJobPostId",
                principalTable: "CompanyJobPosts",
                principalColumn: "Id");
        }
    }
}
