using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class DeleteJobSubcategoriesAndPutAllInJobCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_JobSubcategories_JobSubcategoryId",
                table: "UserJobPosts");

            migrationBuilder.DropTable(
                name: "JobSubcategories");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_JobSubcategoryId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "JobSubcategoryId",
                table: "UserJobPosts");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "JobCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_ParentId",
                table: "JobCategories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCategories_JobCategories_ParentId",
                table: "JobCategories",
                column: "ParentId",
                principalTable: "JobCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCategories_JobCategories_ParentId",
                table: "JobCategories");

            migrationBuilder.DropIndex(
                name: "IX_JobCategories_ParentId",
                table: "JobCategories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "JobCategories");

            migrationBuilder.AddColumn<int>(
                name: "JobSubcategoryId",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JobSubcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSubcategories_JobCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_JobSubcategoryId",
                table: "UserJobPosts",
                column: "JobSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSubcategories_CategoryId",
                table: "JobSubcategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_JobSubcategories_JobSubcategoryId",
                table: "UserJobPosts",
                column: "JobSubcategoryId",
                principalTable: "JobSubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
