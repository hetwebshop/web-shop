using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class ChangingColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skills",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "ExpectedSalary",
                table: "UserJobPosts",
                newName: "Price");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "UserJobPosts",
                newName: "ExpectedSalary");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
