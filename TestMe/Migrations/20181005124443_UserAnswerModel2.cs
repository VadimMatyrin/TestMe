using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class UserAnswerModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "UserAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_AppUserId",
                table: "UserAnswers",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AspNetUsers_AppUserId",
                table: "UserAnswers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AspNetUsers_AppUserId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_AppUserId",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "UserAnswers");
        }
    }
}
