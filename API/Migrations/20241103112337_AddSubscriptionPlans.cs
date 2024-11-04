using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddSubscriptionPlans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdDuration",
                table: "UserJobPosts");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "UserJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdActiveDays = table.Column<int>(type: "int", nullable: false),
                    PriceInCredits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_SubscriptionPlanId",
                table: "UserJobPosts",
                column: "SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_SubscriptionPlanId",
                table: "UserJobPosts",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_SubscriptionPlans_SubscriptionPlanId",
                table: "UserJobPosts");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_SubscriptionPlanId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "UserJobPosts");

            migrationBuilder.AddColumn<int>(
                name: "AdDuration",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
