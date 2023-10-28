using System;

namespace SmartAdmin.WebUI.Models.admin
{
    public class SendEmailModel
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IncludeFather { get; set; }
        public bool IncludeMother { get; set; }
    }
}
