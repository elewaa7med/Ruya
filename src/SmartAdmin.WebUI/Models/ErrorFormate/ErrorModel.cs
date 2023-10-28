using System.Collections.Generic;

namespace SmartAdmin.WebUI.Models.ErrorFormate
{
    public class ErrorModel
    {
        public string page { get; set; }
        public List<string> Error { get; set; }
    }
}
