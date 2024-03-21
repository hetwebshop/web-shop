using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddUserJobCategoriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_UserAddresses_AddressId",
                table: "UserJobPosts");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_AddressId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "UserJobPosts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApplicantDateOfBirth",
                table: "UserJobPosts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "UserJobCategories",
                columns: table => new
                {
                    UserJobPostId = table.Column<int>(type: "int", nullable: false),
                    JobCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobCategories", x => new { x.UserJobPostId, x.JobCategoryId });
                    table.ForeignKey(
                        name: "FK_UserJobCategories_JobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobCategories_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_CityId",
                table: "UserJobPosts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobCategories_JobCategoryId",
                table: "UserJobCategories",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_Cities_CityId",
                table: "UserJobPosts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_Cities_CityId",
                table: "UserJobPosts");

            migrationBuilder.DropTable(
                name: "UserJobCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_CityId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "UserJobPosts");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantDateOfBirth",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_AddressId",
                table: "UserJobPosts",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_UserAddresses_AddressId",
                table: "UserJobPosts",
                column: "AddressId",
                principalTable: "UserAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
