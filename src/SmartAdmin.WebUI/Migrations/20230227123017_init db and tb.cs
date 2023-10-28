using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class initdbandtb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuyaDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    path = table.Column<string>(nullable: true),
                    displayNameToUser = table.Column<string>(nullable: true),
                    displayNameToUserAr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuyaDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TuitionPaymentPlan = table.Column<string>(nullable: true),
                    TuitionPaymentPlanImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Father_IdORIqama = table.Column<string>(nullable: true),
                    FatherNationality = table.Column<int>(nullable: true),
                    FatherRegion = table.Column<int>(nullable: true),
                    FatherFirstNameEnglish = table.Column<string>(nullable: true),
                    FatherFirstNameArabic = table.Column<string>(nullable: true),
                    FatherMiddleNameEnglish = table.Column<string>(nullable: true),
                    FatherMiddleNameArabic = table.Column<string>(nullable: true),
                    FatherFamilyNameEnglish = table.Column<string>(nullable: true),
                    FatherFamilyNameArabic = table.Column<string>(nullable: true),
                    FatherQualification = table.Column<string>(nullable: true),
                    FatherOccupation = table.Column<string>(nullable: true),
                    FatherPlaceOfWork = table.Column<string>(nullable: true),
                    FatherWorkNumber = table.Column<string>(nullable: true),
                    FatherMobileNumber = table.Column<string>(nullable: true),
                    FatherEmailAddress = table.Column<string>(nullable: true),
                    Mother_IdORIqama = table.Column<string>(nullable: true),
                    MotherNationality = table.Column<int>(nullable: true),
                    MotherRegion = table.Column<int>(nullable: true),
                    MotherFirstNameEnglish = table.Column<string>(nullable: true),
                    MotherMiddleNameEnglish = table.Column<string>(nullable: true),
                    MotherFamilyNameEnglish = table.Column<string>(nullable: true),
                    MotherQualification = table.Column<string>(nullable: true),
                    MotherOccupation = table.Column<string>(nullable: true),
                    MotherPlaceOfWork = table.Column<string>(nullable: true),
                    MotherWorkNumber = table.Column<string>(nullable: true),
                    MotherMobileNumber = table.Column<string>(nullable: true),
                    MotherEmailAddress = table.Column<string>(nullable: true),
                    submited = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    enrollment = table.Column<bool>(nullable: true),
                    Submited = table.Column<bool>(nullable: false),
                    Student_IdORIqama = table.Column<string>(nullable: true),
                    StudentFirstNameEnglish = table.Column<string>(nullable: true),
                    StudentFirstNameArabic = table.Column<string>(nullable: true),
                    StudentBirthDate = table.Column<DateTime>(nullable: true),
                    StudentCurrentLevel = table.Column<int>(nullable: true),
                    StudentUpcomingSchoolLevel = table.Column<int>(nullable: true),
                    StudentFirstLanguage = table.Column<string>(nullable: true),
                    StudentCurrentSchool = table.Column<string>(nullable: true),
                    SchoolSystemCurrentlyForStudent = table.Column<int>(nullable: true),
                    HasSiblingsAtRuya = table.Column<int>(nullable: true),
                    SufferFromPreviousFactor = table.Column<byte>(nullable: true),
                    SufferFromPreviousFactorValue = table.Column<string>(nullable: true),
                    SepecialEducation = table.Column<byte>(nullable: true),
                    SepecialEducationValue = table.Column<string>(nullable: true),
                    SkipeedRepeatedGrade = table.Column<byte>(nullable: true),
                    SkipeedRepeatedGradeValue = table.Column<string>(nullable: true),
                    RepecialNeed = table.Column<byte>(nullable: true),
                    RepecialNeedValue = table.Column<string>(nullable: true),
                    ReceivedAnyAward = table.Column<byte>(nullable: true),
                    ReceivedAnyAwardValue = table.Column<string>(nullable: true),
                    TuitionPaymentMethods = table.Column<int>(nullable: true),
                    EmergencyContract1FullName = table.Column<string>(nullable: true),
                    EmergencyContract1RelationShip = table.Column<string>(nullable: true),
                    EmergencyContract1RelationPhoneNumber = table.Column<string>(nullable: true),
                    EmergencyContract2FullName = table.Column<string>(nullable: true),
                    EmergencyContract2RelationShip = table.Column<string>(nullable: true),
                    EmergencyContract2RelationPhoneNumber = table.Column<string>(nullable: true),
                    BirthCertificatePath = table.Column<string>(nullable: true),
                    FamilyNationIDorFatherIqamaFronPath = table.Column<string>(nullable: true),
                    FamilyNationIDorMotherIqamabackPath = table.Column<string>(nullable: true),
                    StudentImmunizationRecordPath = table.Column<string>(nullable: true),
                    StudentPassportPath = table.Column<string>(nullable: true),
                    StudentMostGradeTranscriptPath = table.Column<string>(nullable: true),
                    StudentmedicalClearanceCertificatePath = table.Column<string>(nullable: true),
                    Student64PhotoPath = table.Column<string>(nullable: true),
                    RuyaschoolAdministrationalFeesPath = table.Column<string>(nullable: true),
                    FahterPassportPath = table.Column<string>(nullable: true),
                    MotherPassportPath = table.Column<string>(nullable: true),
                    StudentIqamaPath = table.Column<string>(nullable: true),
                    DocusAuthentic = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: false),
                    TuitionPlanId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicants_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applicants_TuitionPlans_TuitionPlanId",
                        column: x => x.TuitionPlanId,
                        principalTable: "TuitionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applicants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ParentId",
                table: "Applicants",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_TuitionPlanId",
                table: "Applicants",
                column: "TuitionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_UserId",
                table: "Applicants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_UserId",
                table: "Parents",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RuyaDocuments");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "TuitionPlans");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
