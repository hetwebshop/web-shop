using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RemoveAiAnalysisProgressAndIsIncludedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAnalysisProgress",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "IsAiAnalysisIncluded",
                table: "CompanyJobPosts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AiAnalysisProgress",
                table: "CompanyJobPosts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiAnalysisIncluded",
                table: "CompanyJobPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
