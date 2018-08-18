using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class ChangedTestAnswQuest2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "TestQuestion",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "TestAnswer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestion_AppUserId",
                table: "TestQuestion",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAnswer_AppUserId",
                table: "TestAnswer",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswer_AspNetUsers_AppUserId",
                table: "TestAnswer",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_AspNetUsers_AppUserId",
                table: "TestQuestion",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswer_AspNetUsers_AppUserId",
                table: "TestAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_AspNetUsers_AppUserId",
                table: "TestQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestQuestion_AppUserId",
                table: "TestQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAnswer_AppUserId",
                table: "TestAnswer");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "TestQuestion");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "TestAnswer");
        }
    }
}
