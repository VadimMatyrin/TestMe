using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class TestВгкфешщт : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TestDuration",
                table: "Tests",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestDuration",
                table: "Tests");
        }
    }
}
