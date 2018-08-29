using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestMe.Migrations
{
    public partial class TestResultTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "TestResults",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishTime",
                table: "TestResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "TestResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishTime",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TestResults");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "TestResults",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
