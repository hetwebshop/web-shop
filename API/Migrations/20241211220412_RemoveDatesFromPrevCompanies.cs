using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class RemoveDatesFromPrevCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "UserPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "UserPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ApplicantPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ApplicantPreviousCompanies");

            migrationBuilder.AddColumn<int>(
                name: "EndYear",
                table: "UserPreviousCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartYear",
                table: "UserPreviousCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndYear",
                table: "ApplicantPreviousCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartYear",
                table: "ApplicantPreviousCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "UserPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "StartYear",
                table: "UserPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "ApplicantPreviousCompanies");

            migrationBuilder.DropColumn(
                name: "StartYear",
                table: "ApplicantPreviousCompanies");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "UserPreviousCompanies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "UserPreviousCompanies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ApplicantPreviousCompanies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ApplicantPreviousCompanies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
