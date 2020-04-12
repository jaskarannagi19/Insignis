using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class correctedseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "IllustrationDetails",
                columns: new[] { "Id", "AnnualGrossInterestEarned", "AnnualNetInterestEarned", "ClientName", "ClientType", "Comment", "Currency", "EasyAccess", "GenerateDate", "GrossAverageYield", "IllustrationUniqueReference", "NetAverageYield", "NineMonths", "OneMonth", "OneYear", "PartnerEmail", "PartnerName", "PartnerOrganisation", "ReferredBy", "SixMonths", "Status", "ThreeMonths", "ThreeYearsPlus", "TotalDeposit", "TwoYears" },
                values: new object[] { 1, 0m, 0m, " ", 0, null, "GBP", 0.0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "ICS-20200218-199999", 0m, 0.0, 0.0, 0.0, " ", " ", " ", null, 0.0, "Invalid", 0.0, 0.0, 0.0, 0.0 });

            migrationBuilder.InsertData(
                table: "InvestmentTermMapper",
                columns: new[] { "Id", "InvestmentTerm", "InvestmentText" },
                values: new object[,]
                {
                    { 27, "Three Years", "3 Year Bond" },
                    { 26, "Two Years", "2 Year Bond" },
                    { 25, "Two Years", "18 Month Bond" },
                    { 24, "One Year", "1 Year Bond" },
                    { 23, "One Year", "11 Month Bond" },
                    { 22, "One Year", "10 Month Bond" },
                    { 21, "Nine Months", "9 Month Bond" },
                    { 20, "Nine Months", "270 Day" },
                    { 19, "Nine Months", "8 Month Bond" },
                    { 18, "Nine Months", "7 Month Bond" },
                    { 17, "Six Months", "185 Day" },
                    { 16, "Six Months", "6 Month Bond" },
                    { 15, "Six Months", "6 Month" },
                    { 14, "Six Months", "5 Month Bond" },
                    { 13, "Six Months", "4 Month Bond" },
                    { 12, "Three Months", "100 Day" },
                    { 11, "Three Months", "95 Day" },
                    { 10, "Three Months", "3 Month Bond" },
                    { 9, "Three Months", "90 Day" },
                    { 8, "Three Months", "2 Month Bond" },
                    { 7, "One Months", "45 Day" },
                    { 6, "One Month", "30 Day" },
                    { 5, "One Month", "1 Month Bond" },
                    { 4, "One Month", "1 Week Bond" },
                    { 3, "One Month", "1 Week" },
                    { 2, "One Month", "35 Day" },
                    { 1, "Instant Access", "Instant Access" },
                    { 28, "Three Years", "4 Year Bond" },
                    { 29, "Three Years", "5 Year Bond" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IllustrationDetails",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "InvestmentTermMapper",
                keyColumn: "Id",
                keyValue: 29);
        }
    }
}
