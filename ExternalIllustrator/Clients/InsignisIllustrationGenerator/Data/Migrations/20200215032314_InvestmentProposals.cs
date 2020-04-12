using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class InvestmentProposals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdviserName",
                table: "IllustrationDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GenerateDate",
                table: "IllustrationDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IllustrationUniqueReference",
                table: "IllustrationDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "IllustrationDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProposedPortfolio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IllustrationID = table.Column<int>(nullable: false),
                    InstitutionID = table.Column<int>(nullable: true),
                    InstitutionName = table.Column<string>(nullable: true),
                    InstitutionShortName = table.Column<string>(nullable: true),
                    DepositSize = table.Column<decimal>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    InvestmentTerm = table.Column<string>(nullable: true),
                    AnnualInterest = table.Column<decimal>(nullable: false),
                    IllustrationDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposedPortfolio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposedPortfolio_IllustrationDetails_IllustrationDetailId",
                        column: x => x.IllustrationDetailId,
                        principalTable: "IllustrationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProposedPortfolio_IllustrationDetailId",
                table: "ProposedPortfolio",
                column: "IllustrationDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposedPortfolio");

            migrationBuilder.DropColumn(
                name: "AdviserName",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "GenerateDate",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "IllustrationUniqueReference",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IllustrationDetails");
        }
    }
}
