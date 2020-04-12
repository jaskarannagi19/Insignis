using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class InterestFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnnualGrossInterestEarned",
                table: "IllustrationDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualNetInterestEarned",
                table: "IllustrationDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossAverageYield",
                table: "IllustrationDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetAverageYield",
                table: "IllustrationDetails",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualGrossInterestEarned",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "AnnualNetInterestEarned",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "GrossAverageYield",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "NetAverageYield",
                table: "IllustrationDetails");
        }
    }
}
