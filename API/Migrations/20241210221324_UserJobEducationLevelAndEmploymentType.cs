using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UserJobEducationLevelAndEmploymentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationLevelId",
                table: "UserJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentTypeId",
                table: "UserJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_EducationLevelId",
                table: "UserJobPosts",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_EmploymentTypeId",
                table: "UserJobPosts",
                column: "EmploymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_EducationLevels_EducationLevelId",
                table: "UserJobPosts",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_EmploymentTypes_EmploymentTypeId",
                table: "UserJobPosts",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_EducationLevels_EducationLevelId",
                table: "UserJobPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_EmploymentTypes_EmploymentTypeId",
                table: "UserJobPosts");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_EducationLevelId",
                table: "UserJobPosts");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_EmploymentTypeId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "EmploymentTypeId",
                table: "UserJobPosts");
        }
    }
}
