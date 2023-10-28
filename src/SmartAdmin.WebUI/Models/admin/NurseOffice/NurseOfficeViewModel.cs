using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.admin.NurseOffice
{
    public class NurseOfficeViewModel
    {
        public Guid Id { get; set; }
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        public string StudentmedicalClearanceCertificatePath { get; set; }
        public string BirthCertificatePath { get; set; }
        public string StudentImmunizationRecordPath { get; set; }
        public Parent Parent { get; set; }
    }
}
