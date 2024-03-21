using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class ApplicantFNLNInUserJobPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_AspNetUsers_UserId",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserJobPosts",
                newName: "SubmittingUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserJobPosts_UserId",
                table: "UserJobPosts",
                newName: "IX_UserJobPosts_SubmittingUserId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantFirstName",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantLastName",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_AspNetUsers_SubmittingUserId",
                table: "UserJobPosts",
                column: "SubmittingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_AspNetUsers_SubmittingUserId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "ApplicantFirstName",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "ApplicantLastName",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "SubmittingUserId",
                table: "UserJobPosts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserJobPosts_SubmittingUserId",
                table: "UserJobPosts",
                newName: "IX_UserJobPosts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_AspNetUsers_UserId",
                table: "UserJobPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
