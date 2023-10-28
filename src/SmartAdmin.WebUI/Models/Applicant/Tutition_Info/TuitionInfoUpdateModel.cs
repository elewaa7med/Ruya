using System;
using System.ComponentModel.DataAnnotations;
using SmartAdmin.WebUI.Extensions;

namespace SmartAdmin.WebUI.Models.Tuition_Info
{
    public class TuitionInfoUpdateModel
    {
        public Guid Id { get; set; }
        [EnumDataType(typeof(TuitionPaymentMethods))]
        public int? TuitionPaymentMethods { get; set; }
        public string TuitionPlanId { get; set; }
        public bool Submited { get; set; }
    }
}
