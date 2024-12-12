using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class EducationLevelTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_EducationLevel_EducationLevelId",
                table: "CompanyJobPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationLevel",
                table: "EducationLevel");

            migrationBuilder.RenameTable(
                name: "EducationLevel",
                newName: "EducationLevels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationLevels",
                table: "EducationLevels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_EducationLevels_EducationLevelId",
                table: "CompanyJobPosts",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_EducationLevels_EducationLevelId",
                table: "CompanyJobPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationLevels",
                table: "EducationLevels");

            migrationBuilder.RenameTable(
                name: "EducationLevels",
                newName: "EducationLevel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationLevel",
                table: "EducationLevel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_EducationLevel_EducationLevelId",
                table: "CompanyJobPosts",
                column: "EducationLevelId",
                principalTable: "EducationLevel",
                principalColumn: "Id");
        }
    }
}
