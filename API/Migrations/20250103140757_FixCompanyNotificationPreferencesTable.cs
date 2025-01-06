using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class FixCompanyNotificationPreferencesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailEnabled",
                table: "CompanyNotificationPreferences");

            migrationBuilder.RenameColumn(
                name: "InAppEnabled",
                table: "CompanyNotificationPreferences",
                newName: "IsEnabled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "CompanyNotificationPreferences",
                newName: "InAppEnabled");

            migrationBuilder.AddColumn<bool>(
                name: "EmailEnabled",
                table: "CompanyNotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
