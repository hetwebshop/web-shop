using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddEmploymentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmploymentTypeId",
                table: "CompanyJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_EmploymentTypeId",
                table: "CompanyJobPosts",
                column: "EmploymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_EmploymentTypes_EmploymentTypeId",
                table: "CompanyJobPosts",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_EmploymentTypes_EmploymentTypeId",
                table: "CompanyJobPosts");

            migrationBuilder.DropTable(
                name: "EmploymentTypes");

            migrationBuilder.DropIndex(
                name: "IX_CompanyJobPosts_EmploymentTypeId",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "EmploymentTypeId",
                table: "CompanyJobPosts");
        }
    }
}
