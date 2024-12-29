using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UserApplications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmittingUserId = table.Column<int>(type: "int", nullable: false),
                    CompanyAdId = table.Column<int>(type: "int", nullable: false),
                    CompanyJobPostId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CvFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CvFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplications_AspNetUsers_SubmittingUserId",
                        column: x => x.SubmittingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserApplications_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserApplications_CompanyJobPosts_CompanyJobPostId",
                        column: x => x.CompanyJobPostId,
                        principalTable: "CompanyJobPosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserApplicationEducations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationStartYear = table.Column<int>(type: "int", nullable: false),
                    EducationEndYear = table.Column<int>(type: "int", nullable: true),
                    UserApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplicationEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplicationEducations_UserApplications_UserApplicationId",
                        column: x => x.UserApplicationId,
                        principalTable: "UserApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserApplicationPreviousCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserApplicationId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplicationPreviousCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplicationPreviousCompanies_UserApplications_UserApplicationId",
                        column: x => x.UserApplicationId,
                        principalTable: "UserApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationEducations_UserApplicationId",
                table: "UserApplicationEducations",
                column: "UserApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationPreviousCompanies_UserApplicationId",
                table: "UserApplicationPreviousCompanies",
                column: "UserApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_CityId",
                table: "UserApplications",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_CompanyJobPostId",
                table: "UserApplications",
                column: "CompanyJobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_SubmittingUserId",
                table: "UserApplications",
                column: "SubmittingUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserApplicationEducations");

            migrationBuilder.DropTable(
                name: "UserApplicationPreviousCompanies");

            migrationBuilder.DropTable(
                name: "UserApplications");
        }
    }
}
