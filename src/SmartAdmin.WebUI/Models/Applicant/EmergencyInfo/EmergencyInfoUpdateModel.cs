using System;

namespace SmartAdmin.WebUI.Models.EmergencyInfo
{
    public class EmergencyInfoUpdateModel
    {
        public Guid Id { get; set; }
        public string EmergencyContract1FullName { get; set; }
        public string EmergencyContract1RelationShip { get; set; }
        public string EmergencyContract1RelationPhoneNumber { get; set; }
        public string EmergencyContract2FullName { get; set; }
        public string EmergencyContract2RelationShip { get; set; }
        public string EmergencyContract2RelationPhoneNumber { get; set; }
        public bool Submited { get; set; }
    }
}
