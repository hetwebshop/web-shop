using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UserApplicationAIMatchingColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIMatchingDescription",
                table: "UserApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AIMatchingEducationLevel",
                table: "UserApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AIMatchingExperience",
                table: "UserApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AIMatchingResult",
                table: "UserApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AIMatchingSkills",
                table: "UserApplications",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIMatchingDescription",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIMatchingEducationLevel",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIMatchingExperience",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIMatchingResult",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIMatchingSkills",
                table: "UserApplications");
        }
    }
}
