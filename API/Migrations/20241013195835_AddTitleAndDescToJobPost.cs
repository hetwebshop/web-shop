using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddTitleAndDescToJobPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdAdditionalDescription",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdTitle",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdAdditionalDescription",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "AdTitle",
                table: "UserJobPosts");
        }
    }
}
