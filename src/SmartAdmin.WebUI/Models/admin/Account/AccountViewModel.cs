using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.admin.Account
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        [EnumDataType(typeof(boolean))]
        public int? HasSiblingsAtRuya { get; set; }
        [EnumDataType(typeof(Nationality))]
        public int? FatherNationality { get; set; }
        [EnumDataType(typeof(UpcomingSchoolYear))]
        public int? StudentUpcomingSchoolLevel { get; set; }
        [EnumDataType(typeof(TuitionPaymentMethods))]
        public int? TuitionPaymentMethods { get; set; }
        public string RuyaschoolAdministrationalFeesPath { get; set; }
        public string BirthCertificatePath { get; set; }
        public int Status { get; set; }
        public Parent Parent { get; set; }
        public TuitionPlan TuitionPlan { get; set; }
    }
}
