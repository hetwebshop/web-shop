using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class JobPostRefreshInterval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefreshIntervalInDays",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefreshIntervalInDays",
                table: "CompanyJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshIntervalInDays",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "RefreshIntervalInDays",
                table: "CompanyJobPosts");
        }
    }
}
