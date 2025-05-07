using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddApplyViaExternalPlatformColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyViaExternalPlatform",
                table: "CompanyJobPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ExternalPlatformApplicationUrl",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyViaExternalPlatform",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "ExternalPlatformApplicationUrl",
                table: "CompanyJobPosts");
        }
    }
}
