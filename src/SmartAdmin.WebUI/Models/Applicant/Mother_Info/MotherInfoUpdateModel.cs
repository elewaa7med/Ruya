using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.Mother_Info
{
    public class MotherInfoUpdateModel
    {
        public Guid Id { get; set; }
        public string Mother_IdORIqama { get; set; }
        [EnumDataType(typeof(Nationality))]
        public int? MotherNationality { get; set; }
        [EnumDataType(typeof(Religion))]
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
        public Guid ApplicantId { get; set; }

        public bool Submited { get; set; }
    }
}
