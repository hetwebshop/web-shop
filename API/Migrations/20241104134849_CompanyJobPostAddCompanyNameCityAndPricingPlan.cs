using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CompanyJobPostAddCompanyNameCityAndPricingPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCity",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CompanyJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PricingPlanId",
                table: "CompanyJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobPosts_PricingPlanId",
                table: "CompanyJobPosts",
                column: "PricingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_PricingPlans_PricingPlanId",
                table: "CompanyJobPosts",
                column: "PricingPlanId",
                principalTable: "PricingPlans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_PricingPlans_PricingPlanId",
                table: "CompanyJobPosts");

            migrationBuilder.DropIndex(
                name: "IX_CompanyJobPosts_PricingPlanId",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "CompanyCity",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CompanyJobPosts");

            migrationBuilder.DropColumn(
                name: "PricingPlanId",
                table: "CompanyJobPosts");
        }
    }
}
