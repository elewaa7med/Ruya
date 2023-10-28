using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.Dashboard
{
    public class ApplicantDashboardViewModel
    {
        public Guid Id { get; set; }
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public string StudentFirstNameArabic { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        [EnumDataType(typeof(Grade))]
        public int? StudentCurrentLevel { get; set; }
        public bool Submited { get; set; }

    }
}
