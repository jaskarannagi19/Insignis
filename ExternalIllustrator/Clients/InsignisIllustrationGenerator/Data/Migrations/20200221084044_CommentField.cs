using Microsoft.EntityFrameworkCore.Migrations;

namespace InsignisIllustrationGenerator.Data.Migrations
{
    public partial class CommentField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "IllustrationDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferredBy",
                table: "IllustrationDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "IllustrationDetails");

            migrationBuilder.DropColumn(
                name: "ReferredBy",
                table: "IllustrationDetails");
        }
    }
}
