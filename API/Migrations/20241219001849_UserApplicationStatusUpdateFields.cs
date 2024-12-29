using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UserApplicationStatusUpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationLevelId",
                table: "UserApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "UserApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlineMeeting",
                table: "UserApplications",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeetingDateTime",
                table: "UserApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnlineMeetingLink",
                table: "UserApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "UserApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_EducationLevelId",
                table: "UserApplications",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_EducationLevels_EducationLevelId",
                table: "UserApplications",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_EducationLevels_EducationLevelId",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_EducationLevelId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "IsOnlineMeeting",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "MeetingDateTime",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "OnlineMeetingLink",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "UserApplications");
        }
    }
}
