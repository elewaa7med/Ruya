using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.Father_Info
{
    public class FatherInfoUpdateModel
    {
        public Guid Id { get; set; }
        public string Father_IdORIqama { get; set; }
        [EnumDataType(typeof(Nationality))]
        public int? FatherNationality { get; set; }
        [EnumDataType(typeof(Religion))]
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
        public Guid ApplicantId { get; set; }
        public bool Submited { get; set; }
    }
}
