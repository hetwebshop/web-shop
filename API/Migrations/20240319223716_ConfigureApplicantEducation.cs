using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class ConfigureApplicantEducation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantEducation_UserJobPosts_UserJobPostId",
                table: "ApplicantEducation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantEducation",
                table: "ApplicantEducation");

            migrationBuilder.RenameTable(
                name: "ApplicantEducation",
                newName: "ApplicantEducations");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantEducation_UserJobPostId",
                table: "ApplicantEducations",
                newName: "IX_ApplicantEducations_UserJobPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantEducations",
                table: "ApplicantEducations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantEducations_UserJobPosts_UserJobPostId",
                table: "ApplicantEducations",
                column: "UserJobPostId",
                principalTable: "UserJobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantEducations_UserJobPosts_UserJobPostId",
                table: "ApplicantEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantEducations",
                table: "ApplicantEducations");

            migrationBuilder.RenameTable(
                name: "ApplicantEducations",
                newName: "ApplicantEducation");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantEducations_UserJobPostId",
                table: "ApplicantEducation",
                newName: "IX_ApplicantEducation_UserJobPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantEducation",
                table: "ApplicantEducation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantEducation_UserJobPosts_UserJobPostId",
                table: "ApplicantEducation",
                column: "UserJobPostId",
                principalTable: "UserJobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
