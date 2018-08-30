using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class Imprvmnts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "TestAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAnswers_TestId",
                table: "TestAnswers",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswers_Tests_TestId",
                table: "TestAnswers",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswers_Tests_TestId",
                table: "TestAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TestAnswers_TestId",
                table: "TestAnswers");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "TestAnswers");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }
    }
}
