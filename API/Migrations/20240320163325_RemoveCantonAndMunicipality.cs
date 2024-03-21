using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RemoveCantonAndMunicipality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Cantons_CantonId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_Municipalities_MunicipalityId",
                table: "UserAddresses");

            migrationBuilder.DropTable(
                name: "Cantons");

            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_MunicipalityId",
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CantonId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "CantonId",
                table: "Cities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MunicipalityId",
                table: "UserAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantonId",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cantons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cantons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cantons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipalities_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_MunicipalityId",
                table: "UserAddresses",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CantonId",
                table: "Cities",
                column: "CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_Cantons_CountryId",
                table: "Cantons",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_CityId",
                table: "Municipalities",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Cantons_CantonId",
                table: "Cities",
                column: "CantonId",
                principalTable: "Cantons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_Municipalities_MunicipalityId",
                table: "UserAddresses",
                column: "MunicipalityId",
                principalTable: "Municipalities",
                principalColumn: "Id");
        }
    }
}
