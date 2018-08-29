using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class TestResults20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_AppUserId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_AppUserId",
                table: "TestResults");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "TestResults",
                newName: "Username");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "TestResults",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "TestResults",
                newName: "AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "TestResults",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

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
    }
}
