using System;
using Microsoft.AspNetCore.Http;

namespace SmartAdmin.WebUI.Models.Documents
{
    public class DocumentUpdateModel
    {
        public Guid Id { get; set; }
        public IFormFile BirthCertificatePathFile { get; set; }
        public IFormFile FamilyNationIDorFatherIqamaFronPathFile { get; set; }
        public IFormFile FamilyNationIDorMotherIqamabackPathFile { get; set; }
        public IFormFile StudentImmunizationRecordPathFile { get; set; }
        public IFormFile StudentPassportPathFile { get; set; }
        public IFormFile StudentMostGradeTranscriptPathFile { get; set; }
        public IFormFile StudentmedicalClearanceCertificatePathFile { get; set; }
        public IFormFile Student64PhotoPathFile { get; set; }
        public IFormFile RuyaschoolAdministrationalFeesPathFile { get; set; }
        public IFormFile FahterPassportPathFile { get; set; }
        public IFormFile MotherPassportPathFile { get; set; }
        public IFormFile StudentIqamaPathFile { get; set; }

        public string BirthCertificatePath { get; set; }
        public string FamilyNationIDorFatherIqamaFronPath { get; set; }
        public string FamilyNationIDorMotherIqamabackPath { get; set; }
        public string StudentImmunizationRecordPath { get; set; }
        public string StudentPassportPath { get; set; }
        public string StudentMostGradeTranscriptPath { get; set; }
        public string StudentmedicalClearanceCertificatePath { get; set; }
        public string Student64PhotoPath { get; set; }
        public string RuyaschoolAdministrationalFeesPath { get; set; }
        public string FahterPassportPath { get; set; }
        public string MotherPassportPath { get; set; }
        public string StudentIqamaPath { get; set; }
        public bool DocusAuthentic { get; set; }
        public bool Submited { get; set; }
    }
}
