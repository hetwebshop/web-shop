using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddColumnsAiAnalysisErrorToCompanyJobPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiAnalysisError",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AiAnalysisHasError",
                table: "CompanyJobPosts",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAnalysisError",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisHasError",
                table: "CompanyJobPosts");
        }
    }
}
