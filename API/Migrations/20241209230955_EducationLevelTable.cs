using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class EducationLevelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "CompanyJobPosts");

            migrationBuilder.AddColumn<int>(
                name: "EducationLevelId",
                table: "CompanyJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducationLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_EducationLevelId",
                table: "CompanyJobPosts",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_EducationLevel_EducationLevelId",
                table: "CompanyJobPosts",
                column: "EducationLevelId",
                principalTable: "EducationLevel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_EducationLevel_EducationLevelId",
                table: "CompanyJobPosts");

            migrationBuilder.DropTable(
                name: "EducationLevel");

            migrationBuilder.DropIndex(
                name: "IX_CompanyJobPosts_EducationLevelId",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "CompanyJobPosts");

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
