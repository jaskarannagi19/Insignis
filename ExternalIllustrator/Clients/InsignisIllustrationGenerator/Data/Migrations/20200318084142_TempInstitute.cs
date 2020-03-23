using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class TempInstitute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempInstitution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<int>(nullable: false),
                    InstitutionName = table.Column<string>(nullable: true),
                    InvestmentTerm = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    PartnerName = table.Column<string>(nullable: true),
                    PartnerEmail = table.Column<string>(nullable: true),
                    PartnerOrganisation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempInstitution", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempInstitution");
        }
    }
}
