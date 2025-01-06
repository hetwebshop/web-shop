using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RemoveNotificationTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyNotificationPreferences_NotificationTypes_NotificationTypeId",
                table: "CompanyNotificationPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationSettings_NotificationTypes_NotificationTypeId",
                table: "UserNotificationSettings");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_UserNotificationSettings_NotificationTypeId",
                table: "UserNotificationSettings");

            migrationBuilder.DropIndex(
                name: "IX_CompanyNotificationPreferences_NotificationTypeId",
                table: "CompanyNotificationPreferences");

            migrationBuilder.RenameColumn(
                name: "NotificationTypeId",
                table: "CompanyNotificationPreferences",
                newName: "NotificationType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationType",
                table: "CompanyNotificationPreferences",
                newName: "NotificationTypeId");

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationSettings_NotificationTypeId",
                table: "UserNotificationSettings",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNotificationPreferences_NotificationTypeId",
                table: "CompanyNotificationPreferences",
                column: "NotificationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyNotificationPreferences_NotificationTypes_NotificationTypeId",
                table: "CompanyNotificationPreferences",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationSettings_NotificationTypes_NotificationTypeId",
                table: "UserNotificationSettings",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
