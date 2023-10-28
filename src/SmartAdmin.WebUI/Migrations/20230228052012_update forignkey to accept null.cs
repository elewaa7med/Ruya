using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class updateforignkeytoacceptnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Parents_ParentId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_TuitionPlans_TuitionPlanId",
                table: "Applicants");

            migrationBuilder.AlterColumn<Guid>(
                name: "TuitionPlanId",
                table: "Applicants",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                table: "Applicants",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Parents_ParentId",
                table: "Applicants",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_TuitionPlans_TuitionPlanId",
                table: "Applicants",
                column: "TuitionPlanId",
                principalTable: "TuitionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Parents_ParentId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_TuitionPlans_TuitionPlanId",
                table: "Applicants");

            migrationBuilder.AlterColumn<Guid>(
                name: "TuitionPlanId",
                table: "Applicants",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                table: "Applicants",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Parents_ParentId",
                table: "Applicants",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_TuitionPlans_TuitionPlanId",
                table: "Applicants",
                column: "TuitionPlanId",
                principalTable: "TuitionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
