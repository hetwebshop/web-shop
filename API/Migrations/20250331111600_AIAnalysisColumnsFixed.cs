using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AIAnalysisColumnsFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAnalysisEndedOn",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisError",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisHasError",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisReservedCredits",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisStartedOn",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "AiAnalysisStatus",
                table: "CompanyJobPosts");

            migrationBuilder.AddColumn<string>(
                name: "AIAnalysisStatus",
                table: "UserApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIAnaylsisError",
                table: "UserApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AIFeatureUnlocked",
                table: "UserApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIAnalysisStatus",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIAnaylsisError",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "AIFeatureUnlocked",
                table: "UserApplications");

            migrationBuilder.AddColumn<DateTime>(
                name: "AiAnalysisEndedOn",
                table: "CompanyJobPosts",
                type: "datetime2",
                nullable: true);

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

            migrationBuilder.AddColumn<double>(
                name: "AiAnalysisReservedCredits",
                table: "CompanyJobPosts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiAnalysisStartedOn",
                table: "CompanyJobPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiAnalysisStatus",
                table: "CompanyJobPosts",
                type: "int",
                nullable: true);
        }
    }
}
