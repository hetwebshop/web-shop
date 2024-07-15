using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddCompanyJobPostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Cities_CityId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_JobCategories_ParentId",
                table: "JobCategories");

            migrationBuilder.DropTable(
                name: "UserJobSubcategories");

            migrationBuilder.DropIndex(
                name: "IX_JobCategories_ParentId",
                table: "JobCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "JobCategories");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.RenameIndex(
                name: "IX_Company_CityId",
                table: "Companies",
                newName: "IX_Companies_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CompanyJobPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SubmittingUserId = table.Column<int>(type: "int", nullable: false),
                    JobTypeId = table.Column<int>(type: "int", nullable: false),
                    JobCategoryId = table.Column<int>(type: "int", nullable: false),
                    JobPostStatusId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AdStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdDuration = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyJobPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyJobPosts_AspNetUsers_SubmittingUserId",
                        column: x => x.SubmittingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyJobPosts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyJobPosts_JobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyJobPosts_JobPostStatuses_JobPostStatusId",
                        column: x => x.JobPostStatusId,
                        principalTable: "JobPostStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyJobPosts_JobTypes_JobTypeId",
                        column: x => x.JobTypeId,
                        principalTable: "JobTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_CityId",
                table: "CompanyJobPosts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_JobCategoryId",
                table: "CompanyJobPosts",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_JobPostStatusId",
                table: "CompanyJobPosts",
                column: "JobPostStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_JobTypeId",
                table: "CompanyJobPosts",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_SubmittingUserId",
                table: "CompanyJobPosts",
                column: "SubmittingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Cities_CityId",
                table: "Companies",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Cities_CityId",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "CompanyJobPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.RenameIndex(
                name: "IX_Companies_CityId",
                table: "Company",
                newName: "IX_Company_CityId");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "JobCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserJobSubcategories",
                columns: table => new
                {
                    UserJobPostId = table.Column<int>(type: "int", nullable: false),
                    JobCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobSubcategories", x => new { x.UserJobPostId, x.JobCategoryId });
                    table.ForeignKey(
                        name: "FK_UserJobSubcategories_JobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobSubcategories_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_ParentId",
                table: "JobCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobSubcategories_JobCategoryId",
                table: "UserJobSubcategories",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Cities_CityId",
                table: "Company",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_JobCategories_ParentId",
                table: "JobCategories",
                column: "ParentId",
                principalTable: "JobCategories",
                principalColumn: "Id");
        }
    }
}
