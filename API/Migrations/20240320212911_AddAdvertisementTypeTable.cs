using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AddAdvertisementTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvertisementTypeId",
                table: "UserJobPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdvertisementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobPosts_AdvertisementTypeId",
                table: "UserJobPosts",
                column: "AdvertisementTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserJobPosts_AdvertisementTypes_AdvertisementTypeId",
                table: "UserJobPosts",
                column: "AdvertisementTypeId",
                principalTable: "AdvertisementTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserJobPosts_AdvertisementTypes_AdvertisementTypeId",
                table: "UserJobPosts");

            migrationBuilder.DropTable(
                name: "AdvertisementTypes");

            migrationBuilder.DropIndex(
                name: "IX_UserJobPosts_AdvertisementTypeId",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "AdvertisementTypeId",
                table: "UserJobPosts");
        }
    }
}
