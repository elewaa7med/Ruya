using System;
using AutoMapper;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Models.Academic_Info;
using SmartAdmin.WebUI.Models.admin.Account;
using SmartAdmin.WebUI.Models.admin.Admission;
using SmartAdmin.WebUI.Models.admin.applicant;
using SmartAdmin.WebUI.Models.admin.Managment;
using SmartAdmin.WebUI.Models.admin.NurseOffice;
using SmartAdmin.WebUI.Models.Dashboard;
using SmartAdmin.WebUI.Models.Documents;
using SmartAdmin.WebUI.Models.EmergencyInfo;
using SmartAdmin.WebUI.Models.Enrollment;
using SmartAdmin.WebUI.Models.Father_Info;
using SmartAdmin.WebUI.Models.Mother_Info;
using SmartAdmin.WebUI.Models.Student_Info;
using SmartAdmin.WebUI.Models.Tuition_Info;

namespace SmartAdmin.WebUI.Profiles
{
    public class ApplicantProfile : Profile
    {
        public ApplicantProfile()
        {
            CreateMap<Applicant, ApplicantDashboardViewModel>();

            CreateMap<Applicant, EnrollmentUpdateModel>();
            CreateMap<EnrollmentUpdateModel, Applicant>().ForMember(x=>x.Parent , opt => opt.Ignore());

            CreateMap<Applicant, StudentInfoUpdateModel>();
            CreateMap<StudentInfoUpdateModel, Applicant>();


            CreateMap<Parent, FatherInfoUpdateModel>();
            CreateMap<FatherInfoUpdateModel, Parent>();
            CreateMap<Parent, MotherInfoUpdateModel>();
            CreateMap<MotherInfoUpdateModel, Parent>();

            CreateMap<Applicant, AcademicInfoUpdateModel>();
            CreateMap<AcademicInfoUpdateModel, Applicant>();

            CreateMap<Applicant, TuitionInfoUpdateModel>();
            CreateMap<TuitionInfoUpdateModel, Applicant>();

            CreateMap<Applicant, EmergencyInfoUpdateModel>();
            CreateMap<EmergencyInfoUpdateModel, Applicant>();

            CreateMap<Applicant, DocumentUpdateModel>();
            CreateMap<DocumentUpdateModel, Applicant>();

            // admin
            CreateMap<Applicant, ApplicantAdminViewModel>();
            CreateMap<Applicant, ManagementViewModel>();
            CreateMap<Applicant, AdmissionViewModel>();
            CreateMap<Applicant, AccountViewModel>();
            CreateMap<Applicant, NurseOfficeViewModel>();

        }
    }
}
