using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class SetJobTypeAndCategoryOptionalOnUserJobPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_JobCategories_JobCategoryId",
                table: "UserJobPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_JobTypes_JobTypeId",
                table: "UserJobPosts");

            migrationBuilder.AlterColumn<int>(
                name: "JobTypeId",
                table: "UserJobPosts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "JobCategoryId",
                table: "UserJobPosts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_JobCategories_JobCategoryId",
                table: "UserJobPosts",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_JobTypes_JobTypeId",
                table: "UserJobPosts",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_JobCategories_JobCategoryId",
                table: "UserJobPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_JobTypes_JobTypeId",
                table: "UserJobPosts");

            migrationBuilder.AlterColumn<int>(
                name: "JobTypeId",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JobCategoryId",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_JobCategories_JobCategoryId",
                table: "UserJobPosts",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_JobTypes_JobTypeId",
                table: "UserJobPosts",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
