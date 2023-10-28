using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Entities
{
    public class TuitionPlan
    {
        [Key]
        public Guid Id { get; set; }
        public string TuitionPaymentPlan { get; set; }
        public string TuitionPaymentPlanImage { get; set; }

        public ICollection<Applicant> TuitionPlans { get; set; }
    }
}
