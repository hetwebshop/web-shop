using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RenameSubscriptionPlanWithPricingPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_SubscriptionPlanId",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "SubscriptionPlanId",
                table: "UserJobPosts",
                newName: "PricingPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_UserJobPosts_SubscriptionPlanId",
                table: "UserJobPosts",
                newName: "IX_UserJobPosts_PricingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_PricingPlanId",
                table: "UserJobPosts",
                column: "PricingPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_PricingPlanId",
                table: "UserJobPosts");

            migrationBuilder.RenameColumn(
                name: "PricingPlanId",
                table: "UserJobPosts",
                newName: "SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_UserJobPosts_PricingPlanId",
                table: "UserJobPosts",
                newName: "IX_UserJobPosts_SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_SubscriptionPlanId",
                table: "UserJobPosts",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }
    }
}
