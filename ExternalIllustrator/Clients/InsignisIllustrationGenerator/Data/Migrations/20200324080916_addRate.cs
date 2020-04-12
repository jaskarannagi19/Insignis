using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class addRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "TempInstitution",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "TempInstitution");
        }
    }
}
