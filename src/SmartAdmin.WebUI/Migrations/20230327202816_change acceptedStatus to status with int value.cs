using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class changeacceptedStatustostatuswithintvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceStatus",
                table: "Applicants");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Applicants",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Applicants");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptanceStatus",
                table: "Applicants",
                type: "bit",
                nullable: true);
        }
    }
}
