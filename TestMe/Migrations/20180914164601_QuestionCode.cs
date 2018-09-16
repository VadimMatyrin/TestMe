using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class QuestionCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCode",
                table: "TestQuestions");

            migrationBuilder.AddColumn<string>(
                name: "CodeText",
                table: "TestQuestions",
                maxLength: 10000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeText",
                table: "TestQuestions");

            migrationBuilder.AddColumn<bool>(
                name: "IsCode",
                table: "TestQuestions",
                nullable: false,
                defaultValue: false);
        }
    }
}
