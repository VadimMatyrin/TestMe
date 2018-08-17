using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class Names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswer_AspNetUsers_AppUserId",
                table: "TestAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswer_TestQuestion_TestQuestionId",
                table: "TestAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_AspNetUsers_AppUserId",
                table: "TestQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestQuestion",
                table: "TestQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestAnswer",
                table: "TestAnswer");

            migrationBuilder.RenameTable(
                name: "TestQuestion",
                newName: "TestQuestions");

            migrationBuilder.RenameTable(
                name: "TestAnswer",
                newName: "TestAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestion_TestId",
                table: "TestQuestions",
                newName: "IX_TestQuestions_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestion_AppUserId",
                table: "TestQuestions",
                newName: "IX_TestQuestions_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAnswer_TestQuestionId",
                table: "TestAnswers",
                newName: "IX_TestAnswers_TestQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAnswer_AppUserId",
                table: "TestAnswers",
                newName: "IX_TestAnswers_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestQuestions",
                table: "TestQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestAnswers",
                table: "TestAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswers_AspNetUsers_AppUserId",
                table: "TestAnswers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswers_TestQuestions_TestQuestionId",
                table: "TestAnswers",
                column: "TestQuestionId",
                principalTable: "TestQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestions_AspNetUsers_AppUserId",
                table: "TestQuestions",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestions_Tests_TestId",
                table: "TestQuestions",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswers_AspNetUsers_AppUserId",
                table: "TestAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAnswers_TestQuestions_TestQuestionId",
                table: "TestAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestions_AspNetUsers_AppUserId",
                table: "TestQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestions_Tests_TestId",
                table: "TestQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestQuestions",
                table: "TestQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestAnswers",
                table: "TestAnswers");

            migrationBuilder.RenameTable(
                name: "TestQuestions",
                newName: "TestQuestion");

            migrationBuilder.RenameTable(
                name: "TestAnswers",
                newName: "TestAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestions_TestId",
                table: "TestQuestion",
                newName: "IX_TestQuestion_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestQuestions_AppUserId",
                table: "TestQuestion",
                newName: "IX_TestQuestion_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAnswers_TestQuestionId",
                table: "TestAnswer",
                newName: "IX_TestAnswer_TestQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAnswers_AppUserId",
                table: "TestAnswer",
                newName: "IX_TestAnswer_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestQuestion",
                table: "TestQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestAnswer",
                table: "TestAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswer_AspNetUsers_AppUserId",
                table: "TestAnswer",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAnswer_TestQuestion_TestQuestionId",
                table: "TestAnswer",
                column: "TestQuestionId",
                principalTable: "TestQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_AspNetUsers_AppUserId",
                table: "TestQuestion",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestion_Tests_TestId",
                table: "TestQuestion",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
