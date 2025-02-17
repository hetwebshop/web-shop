using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class CompanyJobTypeOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
            name: "JobTypeId",
            table: "CompanyJobPosts",
            nullable: true,
            oldClrType: typeof(int),
            oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                    name: "JobTypeId",
                    table: "CompanyJobPosts",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldNullable: true);
        }
    }
}
