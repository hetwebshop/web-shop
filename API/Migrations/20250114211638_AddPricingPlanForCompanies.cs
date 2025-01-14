using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddPricingPlanForCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_PricingPlans_PricingPlanId",
                table: "CompanyJobPosts");

            migrationBuilder.CreateTable(
                name: "PricingPlanCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdActiveDays = table.Column<int>(type: "int", nullable: false),
                    PriceInCredits = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPlanCompanies", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_PricingPlanCompanies_PricingPlanId",
                table: "CompanyJobPosts",
                column: "PricingPlanId",
                principalTable: "PricingPlanCompanies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyJobPosts_PricingPlanCompanies_PricingPlanId",
                table: "CompanyJobPosts");

            migrationBuilder.DropTable(
                name: "PricingPlanCompanies");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyJobPosts_PricingPlans_PricingPlanId",
                table: "CompanyJobPosts",
                column: "PricingPlanId",
                principalTable: "PricingPlans",
                principalColumn: "Id");
        }
    }
}
