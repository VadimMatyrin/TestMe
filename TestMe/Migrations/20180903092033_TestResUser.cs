using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class TestResUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "TestResults");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "TestResults",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_AppUserId",
                table: "TestResults",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_AppUserId",
                table: "TestResults",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_AppUserId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_AppUserId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "TestResults");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "TestResults",
                nullable: false,
                defaultValue: "");
        }
    }
}
