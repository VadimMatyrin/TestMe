using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class AddRequiresTeacherFieldToTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresTeacherToStart",
                table: "Tests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresTeacherToStart",
                table: "Tests");
        }
    }
}
