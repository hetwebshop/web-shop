using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RenameSubscriptionPlanWithPricingPlan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_PricingPlanId",
                table: "UserJobPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlans",
                newName: "PricingPlans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingPlans",
                table: "PricingPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_PricingPlans_PricingPlanId",
                table: "UserJobPosts",
                column: "PricingPlanId",
                principalTable: "PricingPlans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_PricingPlans_PricingPlanId",
                table: "UserJobPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingPlans",
                table: "PricingPlans");

            migrationBuilder.RenameTable(
                name: "PricingPlans",
                newName: "SubscriptionPlans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_PricingPlanId",
                table: "UserJobPosts",
                column: "PricingPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }
    }
}
