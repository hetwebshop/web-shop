using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RenameTitleWithPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "UserJobPosts",
                newName: "Position");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "UserJobPosts",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
