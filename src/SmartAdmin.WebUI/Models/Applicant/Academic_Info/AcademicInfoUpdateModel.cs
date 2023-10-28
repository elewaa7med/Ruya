using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.Academic_Info
{
    public class AcademicInfoUpdateModel
    {
        public Guid Id { get; set; }
        [EnumDataType(typeof(boolean))]
        public byte? SufferFromPreviousFactor { get; set; }
        public string SufferFromPreviousFactorValue { get; set; }
        [EnumDataType(typeof(boolean))]
        public byte? SepecialEducation { get; set; }
        public string SepecialEducationValue { get; set; }
        [EnumDataType(typeof(boolean))]
        public byte? SkipeedRepeatedGrade { get; set; }
        public string SkipeedRepeatedGradeValue { get; set; }
        [EnumDataType(typeof(boolean))]
        public byte? RepecialNeed { get; set; }
        public string RepecialNeedValue { get; set; }
        [EnumDataType(typeof(boolean))]
        public byte? ReceivedAnyAward { get; set; }
        public string ReceivedAnyAwardValue { get; set; }
        public bool Submited { get; set; }

    }
}
