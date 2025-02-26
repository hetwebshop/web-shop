using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AdditionalColumnsAIOnCompanyJobPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AiAnalysisProgress",
                table: "CompanyJobPosts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiAnalysisStartedOn",
                table: "CompanyJobPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiAnalysisIncluded",
                table: "CompanyJobPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAnalysisProgress",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisStartedOn",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "IsAiAnalysisIncluded",
                table: "CompanyJobPosts");
        }
    }
}
