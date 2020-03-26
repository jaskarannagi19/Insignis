using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class SessionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "TempInstitution",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "ExcludedInstitutes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UniqueReferenceId",
                table: "ExcludedInstitutes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "TempInstitution");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "ExcludedInstitutes");

            migrationBuilder.DropColumn(
                name: "UniqueReferenceId",
                table: "ExcludedInstitutes");
        }
    }
}
