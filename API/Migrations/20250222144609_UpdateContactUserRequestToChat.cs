using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class UpdateContactUserRequestToChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactUserRequests");

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserJobPostId = table.Column<int>(type: "int", nullable: true),
                    CompanyJobPostId = table.Column<int>(type: "int", nullable: true),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chat_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chat_CompanyJobPosts_CompanyJobPostId",
                        column: x => x.CompanyJobPostId,
                        principalTable: "CompanyJobPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chat_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_CompanyJobPostId",
                table: "Chat",
                column: "CompanyJobPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_FromUserId",
                table: "Chat",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_UserJobPostId",
                table: "Chat",
                column: "UserJobPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.CreateTable(
                name: "ContactUserRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    UserJobPostId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUserRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactUserRequests_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactUserRequests_UserJobPosts_UserJobPostId",
                        column: x => x.UserJobPostId,
                        principalTable: "UserJobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactUserRequests_FromUserId",
                table: "ContactUserRequests",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUserRequests_UserJobPostId",
                table: "ContactUserRequests",
                column: "UserJobPostId");
        }
    }
}
