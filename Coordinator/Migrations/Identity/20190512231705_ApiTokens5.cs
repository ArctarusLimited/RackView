using Microsoft.EntityFrameworkCore.Migrations;

namespace Coordinator.Migrations.Identity
{
    public partial class ApiTokens5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ApiTokens",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ApiTokens",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ApiTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ApiTokens",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
