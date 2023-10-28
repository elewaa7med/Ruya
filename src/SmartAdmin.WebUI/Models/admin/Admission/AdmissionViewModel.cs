using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.admin.Admission
{
    public class AdmissionViewModel
    {
        public Guid Id { get; set; }
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public string StudentFirstNameArabic { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        [EnumDataType(typeof(UpcomingSchoolYear))]
        public int? StudentUpcomingSchoolLevel { get; set; }
        public string StudentCurrentSchool { get; set; }
        public string BirthCertificatePath { get; set; }
        public string StudentImmunizationRecordPath { get; set; }
        public string StudentMostGradeTranscriptPath { get; set; }
        public string StudentIqamaPath { get; set; }
        public int Status { get; set; }
        public Parent Parent { get; set; }
    }
}
