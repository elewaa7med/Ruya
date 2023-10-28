using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace SmartAdmin.WebUI.Entities
{
    public class Applicant
    {
        [Key]
        public Guid Id { get; set; }
        // Enrollment
        public bool? enrollment { get; set; }
        //complete submited
        public bool Submited { get; set; }
        public int Status { get; set; }
        public bool? Deleted { get; set; }
        // Student Info
        public string Student_IdORIqama { get; set; }
        public string StudentFirstNameEnglish { get; set; }
        public string StudentFirstNameArabic { get; set; }
        public DateTime? StudentBirthDate { get; set; }
        public int? StudentCurrentLevel { get; set; }
        public int? StudentUpcomingSchoolLevel { get; set; }
        public string StudentFirstLanguage { get; set; }
        public string StudentCurrentSchool { get; set; }
        public int? SchoolSystemCurrentlyForStudent { get; set; }
        public int? HasSiblingsAtRuya { get; set; }

        

        //Academic Info

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

        //tuition Info
        public int? TuitionPaymentMethods { get; set; }

        //Emergency Contact Info
        public string EmergencyContract1FullName { get; set; }
        public string EmergencyContract1RelationShip { get; set; }
        public string EmergencyContract1RelationPhoneNumber { get; set; }
        public string EmergencyContract2FullName { get; set; }
        public string EmergencyContract2RelationShip { get; set; }
        public string EmergencyContract2RelationPhoneNumber { get; set; }

        //Documents
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

        
      
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid? ParentId { get; set; }
        [IgnoreDataMember]
        public Parent Parent{ get; set; }

        public Guid? TuitionPlanId { get; set; }
        public TuitionPlan TuitionPlan { get; set; }
    }
}
