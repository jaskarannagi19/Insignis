using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class ProductBankRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(nullable: false),
                    ProductCode = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductType = table.Column<string>(nullable: true),
                    LiquidityType = table.Column<string>(nullable: true),
                    NoticeText = table.Column<string>(nullable: true),
                    NoticeDays = table.Column<string>(nullable: true),
                    TermText = table.Column<string>(nullable: true),
                    TermDays = table.Column<string>(nullable: true),
                    RateFor50KDeposit = table.Column<string>(nullable: true),
                    RateFor100KDeposit = table.Column<string>(nullable: true),
                    RateFor250KDeposit = table.Column<string>(nullable: true),
                    InterestPaid = table.Column<string>(nullable: true),
                    MinimumDeposit = table.Column<string>(nullable: true),
                    MaximumDeposit = table.Column<string>(nullable: true),
                    IsAvailableToPersonalHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToJointHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToSMEHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToTrustHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToIncorporatedCharityHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToPowerOfAttorneyHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToPersonalTrustHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToLocalAuthorityHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToSSASHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToSIPPHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToLargeCorporateHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToUnincorporatedCharityHubAccounts = table.Column<bool>(nullable: false),
                    IsAvailableToCourtOfProtectionHubAccounts = table.Column<bool>(nullable: false),
                    BankID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Bank_BankID",
                        column: x => x.BankID,
                        principalTable: "Bank",
                        principalColumn: "BankID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_BankID",
                table: "Product",
                column: "BankID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
