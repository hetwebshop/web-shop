using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UserAndApplicantPreviousCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationLevelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentTypeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicantPreviousCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserJobPostId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantPreviousCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantPreviousCompanies_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreviousCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreviousCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreviousCompanies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EducationLevelId",
                table: "AspNetUsers",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmploymentTypeId",
                table: "AspNetUsers",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantPreviousCompanies_UserJobPostId",
                table: "ApplicantPreviousCompanies",
                column: "UserJobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreviousCompanies_UserId",
                table: "UserPreviousCompanies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EducationLevels_EducationLevelId",
                table: "AspNetUsers",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmploymentTypes_EmploymentTypeId",
                table: "AspNetUsers",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EducationLevels_EducationLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmploymentTypes_EmploymentTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ApplicantPreviousCompanies");

            migrationBuilder.DropTable(
                name: "UserPreviousCompanies");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EducationLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmploymentTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmploymentTypeId",
                table: "AspNetUsers");
        }
    }
}
