using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class IsUserBanned : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "AspNetUsers");
        }
    }
}
