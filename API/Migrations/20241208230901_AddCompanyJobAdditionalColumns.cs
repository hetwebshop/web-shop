using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddCompanyJobAdditionalColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsRequired",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HowToApply",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxSalary",
                table: "CompanyJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinSalary",
                table: "CompanyJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredExperience",
                table: "CompanyJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequiredSkills",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkEnvironmentDescription",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "DocumentsRequired",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "HowToApply",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "MinSalary",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "RequiredExperience",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "WorkEnvironmentDescription",
                table: "CompanyJobPosts");
        }
    }
}
