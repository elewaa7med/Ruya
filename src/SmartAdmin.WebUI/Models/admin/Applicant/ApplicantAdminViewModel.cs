using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.admin.applicant
{
    public class ApplicantAdminViewModel
    {
        public Guid Id { get; set; }
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public string StudentFirstNameArabic { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        [EnumDataType(typeof(Grade))]
        public int? StudentCurrentLevel { get; set; }
        [EnumDataType(typeof(UpcomingSchoolYear))]
        public int? StudentUpcomingSchoolLevel { get; set; }
        public string StudentFirstLanguage { get; set; }
        public string StudentCurrentSchool { get; set; }
        [EnumDataType(typeof(SchoolSystem))]
        public int? SchoolSystemCurrentlyForStudent { get; set; }
        [EnumDataType(typeof(boolean))]
        public int? HasSiblingsAtRuya { get; set; }
        public bool Submited { get; set; }
        public string RuyaschoolAdministrationalFeesPath { get; set; }
        public int Status { get; set; }

        public Parent Parent { get; set; }
    }
}
