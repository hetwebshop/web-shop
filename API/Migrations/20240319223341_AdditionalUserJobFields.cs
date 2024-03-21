using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class AdditionalUserJobFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicantDateOfBirth",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantEmail",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantGender",
                table: "UserJobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantPhoneNumber",
                table: "UserJobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicantEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationStartYear = table.Column<int>(type: "int", nullable: false),
                    EducationEndYear = table.Column<int>(type: "int", nullable: false),
                    UserJobPostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantEducation_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantEducation_UserJobPostId",
                table: "ApplicantEducation",
                column: "UserJobPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantEducation");

            migrationBuilder.DropColumn(
                name: "ApplicantDateOfBirth",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "ApplicantEmail",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "ApplicantGender",
                table: "UserJobPosts");

            migrationBuilder.DropColumn(
                name: "ApplicantPhoneNumber",
                table: "UserJobPosts");
        }
    }
}
