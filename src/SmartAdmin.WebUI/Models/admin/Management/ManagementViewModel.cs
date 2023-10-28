using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.admin.Managment
{
    public class ManagementViewModel
    {
        public Guid Id { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        [EnumDataType(typeof(Grade))]
        public int? StudentCurrentLevel { get; set; }
        [EnumDataType(typeof(Nationality))]
        public int? FatherNationality { get; set; }
        [EnumDataType(typeof(UpcomingSchoolYear))]
        public int? StudentUpcomingSchoolLevel { get; set; }
        [EnumDataType(typeof(SchoolSystem))]
        public int? SchoolSystemCurrentlyForStudent { get; set; }
        public string StudentCurrentSchool { get; set; }
        [EnumDataType(typeof(boolean))]
        public int? HasSiblingsAtRuya { get; set; }

        public string BirthCertificatePath { get; set; }
        public string FamilyNationIDorFatherIqamaFronPath { get; set; }
        public string StudentMostGradeTranscriptPath { get; set; }

        public byte? SufferFromPreviousFactor { get; set; }
        public string SufferFromPreviousFactorValue { get; set; }
        public byte? SepecialEducation { get; set; }
        public string SepecialEducationValue { get; set; }
        public byte? SkipeedRepeatedGrade { get; set; }
        public string SkipeedRepeatedGradeValue { get; set; }
        public byte? RepecialNeed { get; set; }
        public string RepecialNeedValue { get; set; }
        public byte? ReceivedAnyAward { get; set; }
        public string ReceivedAnyAwardValue { get; set; }

        public bool Submited { get; set; }

        public Parent Parent { get; set; }
    }
}
