using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace SmartAdmin.WebUI.Entities
{
    public class Parent
    {
        [Key]
        public Guid Id { get; set; }
        //Father Info
        public string Father_IdORIqama { get; set; }
        public int? FatherNationality { get; set; }
        public int? FatherRegion { get; set; }
        public string FatherFirstNameEnglish { get; set; }
        public string FatherFirstNameArabic { get; set; }
        public string FatherMiddleNameEnglish { get; set; }
        public string FatherMiddleNameArabic { get; set; }
        public string FatherFamilyNameEnglish { get; set; }
        public string FatherFamilyNameArabic { get; set; }
        public string FatherQualification { get; set; }
        public string FatherOccupation { get; set; }
        public string FatherPlaceOfWork { get; set; }
        public string FatherWorkNumber { get; set; }
        public string FatherMobileNumber { get; set; }
        public string FatherEmailAddress { get; set; }

        //Mother Info
        public string Mother_IdORIqama { get; set; }
        public int? MotherNationality { get; set; }
        public int? MotherRegion { get; set; }
        public string MotherFirstNameEnglish { get; set; }
        public string MotherMiddleNameEnglish { get; set; }
        public string MotherFamilyNameEnglish { get; set; }
        public string MotherQualification { get; set; }
        public string MotherOccupation { get; set; }
        public string MotherPlaceOfWork { get; set; }
        public string MotherWorkNumber { get; set; }
        public string MotherMobileNumber { get; set; }
        public string MotherEmailAddress { get; set; }
        public bool  submited { get; set; }

        [NotMapped]
        public bool Fathermapped { get; set; }
        [NotMapped]
        public bool Mothermapped { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        [IgnoreDataMember]
        public ICollection<Applicant> Applicants { get; set; }

    }
}
