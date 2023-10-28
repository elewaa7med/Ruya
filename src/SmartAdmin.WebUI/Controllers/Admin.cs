using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models.Academic_Info;
using SmartAdmin.WebUI.Models.admin;
using SmartAdmin.WebUI.Models.admin.Account;
using SmartAdmin.WebUI.Models.admin.Admission;
using SmartAdmin.WebUI.Models.admin.applicant;
using SmartAdmin.WebUI.Models.admin.Dashboard;
using SmartAdmin.WebUI.Models.admin.Managment;
using SmartAdmin.WebUI.Models.admin.NurseOffice;
using SmartAdmin.WebUI.Models.Documents;
using SmartAdmin.WebUI.Models.EmergencyInfo;
using SmartAdmin.WebUI.Models.Enrollment;
using SmartAdmin.WebUI.Models.ErrorFormate;
using SmartAdmin.WebUI.Models.Father_Info;
using SmartAdmin.WebUI.Models.Mother_Info;
using SmartAdmin.WebUI.Models.Student_Info;
using SmartAdmin.WebUI.Models.Tuition_Info;
using SmartAdmin.WebUI.Service;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Admin : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<User> _user;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailSender;

        public Admin(ApplicationContext context, IMapper mapper, IFileProvider fileProvider,
            UserManager<User> user, SignInManager<User> signInManager, IEmailService emailSender)
        {
            _context = context;
            _mapper = mapper;
            _fileProvider = fileProvider;
            _user = user;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Dashboard()
        {
            var applicants = _context.Applicants;
            DashboardViewModel dash = new DashboardViewModel();

            dash.TotalApplicants = applicants.Where(x => x.Deleted != true).Count();
            dash.TotalCompletedProfiles = applicants.Where(x => x.Deleted != true && x.Submited == true).Count();
            dash.TotalIncompletedProfiles = applicants.Where(x => x.Deleted != true && x.Submited != true).Count();
            dash.TotalAcceptedApplicants = applicants.Where(x => x.Deleted != true && x.Status == (int)Status.Acceptedfinical).Count();
            dash.TotalRejectedApplicants = applicants.Where(x => x.Deleted != true && x.Status == (int)Status.Rejected).Count();
            dash.TotalAcceptedApplicantsOnWaiting = applicants.Where(x => x.Deleted != true && x.Status == (int)Status.AcceptedWaiting).Count();

            dash.TotalApplicantPreLevel = applicants.Where(x => x.Deleted != true &&
                (x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.PREKG || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG1 || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG2 || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG3)).Count();
            dash.TotalApplicantPreKG = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.PREKG).Count();
            dash.TotalApplicantKG1 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG1).Count();
            dash.TotalApplicantKG2 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG2).Count();
            dash.TotalApplicantKG3 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG3).Count();
            dash.TotalApplicantElementaryLevel = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel > 4).Count();
            dash.TotalApplicantGrade1 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade1).Count();
            dash.TotalApplicantGrade2 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade2).Count();
            dash.TotalApplicantGrade3 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade3).Count();
            dash.TotalApplicantGrade4 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade4).Count();
            dash.TotalApplicantGrade5 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade5).Count();
            dash.TotalApplicantGrade6 = applicants.Where(x => x.Deleted != true && x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.Grade6).Count();

            return View(dash);
        }

        public IActionResult Applicants(string complete, string Grade, int? status)
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Deleted != true);
            if (complete != null)
            {
                if (complete == "true")
                    applicants = applicants.Where(x => x.Submited == true);
                if (complete == "false")
                    applicants = applicants.Where(x => x.Submited != true);
            }

            if (status != null)
            {
                applicants = applicants.Where(x => x.Status == status);
            }

            if (Grade != null)
            {
                if (Grade == "Preschool")
                    applicants = applicants.Where(x => x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.PREKG || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG1 || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG2 || x.StudentUpcomingSchoolLevel == (int)UpcomingSchoolYear.KG3);
                else if (Grade == "elementary")
                    applicants = applicants.Where(x => x.StudentUpcomingSchoolLevel > 4);
                else
                    applicants = applicants.Where(x => x.StudentUpcomingSchoolLevel == int.Parse(Grade));
            }

            List<ApplicantAdminViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<ApplicantAdminViewModel>>(applicants.ToList());
            return View(applicantAdminViewModel);
        }

        public IActionResult Completed_Applicants(int? Grade, int? Nationality, string From, string To, int? Status)
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Submited == true && x.Deleted != true);
            if (Grade != null & Grade != 0)
            {
                ViewBag.Grade = Grade;
                applicants = applicants.Where(x => x.StudentCurrentLevel == Grade);
            }
            if (Nationality != null && Nationality != 0)
            {
                ViewBag.Nationality = Nationality;
                applicants = applicants.Where(x => x.Parent.FatherNationality == Nationality);
            }
            if (Status != null && Status != 0)
            {
                ViewBag.Status = Status;
                applicants = applicants.Where(x => x.Status == Status);
            }
            if (From != null && To != null)
            {
                ViewBag.From = From;
                ViewBag.To = To;
                DateTime dateFrom = DateTime.Parse(From);
                DateTime dateTo = DateTime.Parse(To);
                applicants = applicants.Where(x => x.StudentBirthDate >= dateFrom && x.StudentBirthDate <= dateTo);
            }
            List<ApplicantAdminViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<ApplicantAdminViewModel>>(applicants.ToList());
            return View(applicantAdminViewModel);
        }



        public IActionResult Incompleted_Applicants(int? Grade, int? Nationality, string From, string To)
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Submited == false && x.Deleted != true);
            if (Grade != null & Grade != 0)
            {
                ViewBag.Grade = Grade;
                applicants = applicants.Where(x => x.StudentCurrentLevel == Grade);
            }
            if (Nationality != null && Nationality != 0)
            {
                ViewBag.Nationality = Nationality;
                applicants = applicants.Where(x => x.Parent.FatherNationality == Nationality);
            }
            if (From != null && To != null)
            {
                ViewBag.From = From;
                ViewBag.To = To;
                DateTime dateFrom = DateTime.Parse(From);
                DateTime dateTo = DateTime.Parse(To);
                applicants = applicants.Where(x => x.StudentBirthDate >= dateFrom && x.StudentBirthDate <= dateTo);
            }
            List<ApplicantAdminViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<ApplicantAdminViewModel>>(applicants.ToList());
            return View(applicantAdminViewModel);
        }

        public IActionResult Archived()
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Deleted == true);
            List<ApplicantAdminViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<ApplicantAdminViewModel>>(applicants.ToList());
            return View(applicantAdminViewModel);
        }

        public IActionResult Unarchive(Guid Id)
        {
            var applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Deleted = false;
            _context.Update(applicant);
            _context.SaveChanges();
            return RedirectToAction("Archived");
        }


        public IActionResult Account()
        {
            List<Applicant> applicants = _context.Applicants.Include(x => x.Parent).Include(x => x.TuitionPlan).Where(x => x.Deleted != true).ToList();
            List<AccountViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<AccountViewModel>>(applicants);
            return View(applicantAdminViewModel);
        }

        public IActionResult Admission(int? Grade, int? Status, string From, string To)
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Deleted != true);
            if (Grade != null & Grade != 0)
            {
                ViewBag.Grade = Grade;
                applicants = applicants.Where(x => x.StudentUpcomingSchoolLevel == Grade);
            }
            if (Status != null && Status != 0)
            {
                ViewBag.Status = Status;
                applicants = applicants.Where(x => x.Status == Status);
            }
            if (From != null && To != null)
            {
                ViewBag.From = From;
                ViewBag.To = To;
                DateTime dateFrom = DateTime.Parse(From);
                DateTime dateTo = DateTime.Parse(To);
                applicants = applicants.Where(x => x.StudentBirthDate >= dateFrom && x.StudentBirthDate <= dateTo);
            }
            List<AdmissionViewModel> applicantAdmissionViewModel = _mapper.Map<List<Applicant>, List<AdmissionViewModel>>(applicants.ToList());
            return View(applicantAdmissionViewModel);
        }

        public IActionResult Management()
        {
            List<Applicant> applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Deleted != true).ToList();
            List<ManagementViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<ManagementViewModel>>(applicants);
            return View(applicantAdminViewModel);
        }

        public IActionResult NurseOffice()
        {
            List<Applicant> applicants = _context.Applicants.Include(x => x.Parent).Where(x => x.Deleted != true).ToList();
            List<NurseOfficeViewModel> applicantAdminViewModel = _mapper.Map<List<Applicant>, List<NurseOfficeViewModel>>(applicants);
            return View(applicantAdminViewModel);
        }

        public IActionResult Pending(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Pending;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Stage_Applicant(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Stage2;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Accept_Applicant_finical(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Acceptedfinical;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Accept_Applicant_WaitingList(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.AcceptedWaiting;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Accept_Applicant_conditions_financial(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Acceptedconditions;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Accept_Applicant_recommendation_financial(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Acceptedrecommendation;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction("Completed_Applicants");
        }

        public IActionResult Reject_Applicant(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Status = (int)Status.Rejected;
            _context.Update(applicant);
            _context.SaveChanges();
            NotifyParentWithRejection(applicant);
            return RedirectToAction("Completed_Applicants");
        }

        private void NotifyParentWithRejection(Applicant applicant)
        {
            string fatheremail = applicant.Parent.FatherEmailAddress;
            string motheremail = applicant.Parent.MotherEmailAddress;
            string Title = "خطاب اعتذار عن قبول طالبـ/ـة في المدارس";
            AlternateView body = GetEmbeddedImage(applicant);
            if (!string.IsNullOrEmpty(fatheremail))
            {
                SendCustomeEmailAlternativeAsync(fatheremail, Title, body);

            }
            if (!string.IsNullOrEmpty(motheremail))
            {
                SendCustomeEmailAlternativeAsync(motheremail, Title, body);

            }
        }

        private AlternateView GetEmbeddedImage(Applicant applicant)
        {
            var _fileDeafaultPath = ((PhysicalFileProvider)_fileProvider).Root;
            var empImagePath = Path.Combine(_fileDeafaultPath, "Image\\sign.png");
            LinkedResource res = new LinkedResource(empImagePath, MediaTypeNames.Image.Jpeg);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = $"<p style='font-size:18px;'>من: إدارة مدارس رؤية التعليمية</p>" +
                     $"<p style='font-size:18px;'>إلى:ولي أمر الطالبـ/ـة " + applicant.StudentFirstNameArabic + " " + applicant.Parent.FatherFirstNameArabic + " " + applicant.Parent.FatherMiddleNameArabic + " " + applicant.Parent.FatherFamilyNameArabic + "</p>" +
                     $"<p style='font-size:18px;'>يؤسفنا ابلاغكم بعدم قبول ابنكم/ابنتكم لدينا للعام الدراسي2022-2023م بمرحلة (التمهيدي) ،شاكرين لكم ثقتكم بمدارسنا ومتمنين لابنكم /ابنتكم كل التوفيق.</p>" +
                     $"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public IActionResult SendeEmailToIncompletedApplicant(Guid Id, string from)
        {
            bool FatherEmail = false, MotherEmail = false;
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            if (applicant.Parent.FatherEmailAddress != null)
            {
                FatherEmail = EmailContent(applicant.Parent.FatherEmailAddress);
            }

            if (applicant.Parent.MotherEmailAddress != null)
            {
                MotherEmail = EmailContent(applicant.Parent.MotherEmailAddress);
            }
            ViewBag.FatherEmail = FatherEmail;
            ViewBag.MotherEmail = MotherEmail;
            return RedirectToAction(from);
        }

        public IActionResult Delete(Guid Id, string From)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
                return NotFound();
            applicant.Deleted = true;
            _context.Update(applicant);
            _context.SaveChanges();

            return RedirectToAction(From);
        }

        public IActionResult ExportToExcel(int? Grade, int? Nationality,
            string From, string To, int? Status, bool? Submited, string id)
        {
            var applicants = _context.Applicants.Include(x => x.Parent).Include(x => x.TuitionPlan).Where(x => x.Deleted != true);
            if (Submited != null)
            {
                applicants = applicants.Where(x => x.Submited == Submited);
            }
            if (id != null)
            {
                applicants = applicants.Where(x => x.Id == Guid.Parse(id));
            }
            if (Grade != null & Grade != 0)
            {
                ViewBag.Grade = Grade;
                applicants = applicants.Where(x => x.StudentCurrentLevel == Grade);
            }
            if (Nationality != null && Nationality != 0)
            {
                ViewBag.Nationality = Nationality;
                applicants = applicants.Where(x => x.Parent.FatherNationality == Nationality);
            }
            if (From != null && To != null)
            {
                ViewBag.From = From;
                ViewBag.To = To;
                DateTime dateFrom = DateTime.Parse(From);
                DateTime dateTo = DateTime.Parse(To);
                applicants = applicants.Where(x => x.StudentBirthDate >= dateFrom && x.StudentBirthDate <= dateTo);
            }
            if (Status != null && Status != 0)
            {
                ViewBag.Status = Status;
                applicants = applicants.Where(x => x.Status == Status);
            }

            using (var Workbook = new XLWorkbook())
            {
                var workSheet = Workbook.Worksheets.Add("Students");
                var CurrentRow = 1;

                workSheet.Cell(CurrentRow, 1).Value = "Student’s ID number or Iqama number";
                workSheet.Cell(CurrentRow, 2).Value = "Student’s first name in English";
                workSheet.Cell(CurrentRow, 3).Value = "Student’s first name in Arabic";
                workSheet.Cell(CurrentRow, 4).Value = "Student’s date of birth";
                workSheet.Cell(CurrentRow, 5).Value = "Student’s current level";
                workSheet.Cell(CurrentRow, 6).Value = "Upcoming school year";
                workSheet.Cell(CurrentRow, 7).Value = "Student’s first language";
                workSheet.Cell(CurrentRow, 8).Value = "Student’s current school";
                workSheet.Cell(CurrentRow, 9).Value = "School system currently attended by the student";
                workSheet.Cell(CurrentRow, 10).Value = "Does the student have siblings at Ruya School ? ";

                workSheet.Cell(CurrentRow, 11).Value = "Father’s ID number or Iqama number";
                workSheet.Cell(CurrentRow, 12).Value = "Father’s Nationality";
                workSheet.Cell(CurrentRow, 13).Value = "Father’s religion";
                workSheet.Cell(CurrentRow, 14).Value = "Father’s first name in English";
                workSheet.Cell(CurrentRow, 15).Value = "Father’s middle name in English";
                workSheet.Cell(CurrentRow, 16).Value = "Father’s family name in English";
                workSheet.Cell(CurrentRow, 17).Value = "Father’s first name in Arabic";
                workSheet.Cell(CurrentRow, 18).Value = "Father’s middle name in Arabic";
                workSheet.Cell(CurrentRow, 19).Value = "Father’s family name in Arabic";
                workSheet.Cell(CurrentRow, 20).Value = "Father’s qualification";
                workSheet.Cell(CurrentRow, 21).Value = "Father’s occupation";
                workSheet.Cell(CurrentRow, 22).Value = "Father’s place of work";
                workSheet.Cell(CurrentRow, 23).Value = "Father’s work number";
                workSheet.Cell(CurrentRow, 24).Value = "Father’s mobile number";
                workSheet.Cell(CurrentRow, 25).Value = "Father’s email address";

                workSheet.Cell(CurrentRow, 26).Value = "Mother’s ID number or Iqama number";
                workSheet.Cell(CurrentRow, 27).Value = "Mother’s Nationality";
                workSheet.Cell(CurrentRow, 28).Value = "Mother’s religion";
                workSheet.Cell(CurrentRow, 29).Value = "Mother’s first name in English";
                workSheet.Cell(CurrentRow, 30).Value = "Mother’s middle name in English";
                workSheet.Cell(CurrentRow, 31).Value = "Mother’s family name in English";
                workSheet.Cell(CurrentRow, 32).Value = "Mother’s qualification";
                workSheet.Cell(CurrentRow, 33).Value = "Mother’s occupation";
                workSheet.Cell(CurrentRow, 34).Value = "Mother’s place of work";
                workSheet.Cell(CurrentRow, 35).Value = "Mother’s work number";
                workSheet.Cell(CurrentRow, 36).Value = "Mother’s mobile number";
                workSheet.Cell(CurrentRow, 37).Value = "Mother’s email address";

                workSheet.Cell(CurrentRow, 38).Value = "Does the student suffer from any of the previous factors, if yes please specify";
                workSheet.Cell(CurrentRow, 39).Value = "Has your child received services for learning disorder, special education, or previous program modification?";
                workSheet.Cell(CurrentRow, 40).Value = "Has the applicant skipped or repeated a grade /year? IF yes. which grade/year?";
                workSheet.Cell(CurrentRow, 41).Value = "Are you aware of any special needs (physical, social or academic) your child might have?";
                workSheet.Cell(CurrentRow, 42).Value = "Has the applicant received any award ? IF yes, what ?";

                workSheet.Cell(CurrentRow, 43).Value = "Tuition payment method";
                workSheet.Cell(CurrentRow, 44).Value = "Preferred payment plan";

                workSheet.Cell(CurrentRow, 45).Value = "First Guardian Full name";
                workSheet.Cell(CurrentRow, 46).Value = "First Guardian Relationship to student";
                workSheet.Cell(CurrentRow, 47).Value = "First Guardian Mobile number";
                workSheet.Cell(CurrentRow, 48).Value = "Second Guardian Full name";
                workSheet.Cell(CurrentRow, 49).Value = "Second Guardian Relationship to student";
                workSheet.Cell(CurrentRow, 50).Value = "Second Guardian Mobile number";


                workSheet.Cell(CurrentRow, 51).Value = "Birth certificate";
                workSheet.Cell(CurrentRow, 52).Value = "Family national ID: Front side Or Fathers iqama id for non-saudis";
                workSheet.Cell(CurrentRow, 53).Value = "Family national ID: Back side Or Mother’s iqama id for non-saudis";
                workSheet.Cell(CurrentRow, 54).Value = "Student’s immunization record";
                workSheet.Cell(CurrentRow, 55).Value = "Student’s passport";
                workSheet.Cell(CurrentRow, 56).Value = "Student’s most recent grade transcript";
                workSheet.Cell(CurrentRow, 57).Value = "Medical Clearance certificate";
                workSheet.Cell(CurrentRow, 58).Value = "4*6 photograph";
                workSheet.Cell(CurrentRow, 59).Value = "Ruya school administrational fees receipt";
                workSheet.Cell(CurrentRow, 60).Value = "Father’s passport (for Non Saudis)";
                workSheet.Cell(CurrentRow, 61).Value = "Mother’s passport (for Non Saudis)";
                workSheet.Cell(CurrentRow, 62).Value = "Student’s Iqama (for Non Saudis)";

                foreach (var item in applicants.ToList())
                {
                    CurrentRow++;
                    Enum.TryParse(item.StudentCurrentLevel.ToString(), out Grade StudentCurrentLevel);
                    Enum.TryParse(item.StudentUpcomingSchoolLevel.ToString(), out UpcomingSchoolYear StudentUpcomingSchoolLevel);
                    Enum.TryParse(item.SchoolSystemCurrentlyForStudent.ToString(), out SchoolSystem SchoolSystemCurrentlyForStudent);
                    Enum.TryParse(item.HasSiblingsAtRuya.ToString(), out boolean HasSiblingsAtRuya);
                    Enum.TryParse(item.Parent?.FatherNationality.ToString(), out Nationality FatherNationality);
                    Enum.TryParse(item.Parent?.FatherRegion.ToString(), out Religion FatherReligion);
                    Enum.TryParse(item.Parent?.MotherNationality.ToString(), out Nationality MotherNationality);
                    Enum.TryParse(item.Parent?.MotherRegion.ToString(), out Religion MotherReligion);

                    Enum.TryParse(item.SufferFromPreviousFactor.ToString(), out boolean SufferFromPreviousFactor);
                    Enum.TryParse(item.SepecialEducation.ToString(), out boolean SepecialEducation);
                    Enum.TryParse(item.SkipeedRepeatedGrade.ToString(), out boolean SkipeedRepeatedGrade);
                    Enum.TryParse(item.RepecialNeed.ToString(), out boolean RepecialNeed);
                    Enum.TryParse(item.ReceivedAnyAward.ToString(), out boolean ReceivedAnyAward);
                    Enum.TryParse(item.TuitionPaymentMethods.ToString(), out TuitionPaymentMethods TuitionPaymentMethods);


                    workSheet.Cell(CurrentRow, 1).Value = item.Student_IdORIqama;
                    workSheet.Cell(CurrentRow, 2).Value = item.StudentFirstNameEnglish;
                    workSheet.Cell(CurrentRow, 3).Value = item.StudentFirstNameArabic;
                    workSheet.Cell(CurrentRow, 4).Value = item.StudentBirthDate;
                    workSheet.Cell(CurrentRow, 5).Value = StudentCurrentLevel == 0 ? "" : StudentCurrentLevel.GetAttribute<DisplayAttribute>().Name;
                    workSheet.Cell(CurrentRow, 6).Value = StudentUpcomingSchoolLevel == 0 ? "" : StudentUpcomingSchoolLevel.GetAttribute<DisplayAttribute>().Name;
                    workSheet.Cell(CurrentRow, 7).Value = item.StudentFirstLanguage;
                    workSheet.Cell(CurrentRow, 8).Value = item.StudentCurrentSchool;
                    workSheet.Cell(CurrentRow, 9).Value = SchoolSystemCurrentlyForStudent == 0 ? "" : SchoolSystemCurrentlyForStudent.GetAttribute<DisplayAttribute>().Name;
                    workSheet.Cell(CurrentRow, 10).Value = HasSiblingsAtRuya.ToString();

                    workSheet.Cell(CurrentRow, 11).Value = item.Parent?.Father_IdORIqama;
                    workSheet.Cell(CurrentRow, 12).Value = FatherNationality.ToString();
                    workSheet.Cell(CurrentRow, 13).Value = FatherReligion.ToString();
                    workSheet.Cell(CurrentRow, 14).Value = item.Parent?.FatherFirstNameEnglish;
                    workSheet.Cell(CurrentRow, 15).Value = item.Parent?.FatherMiddleNameEnglish;
                    workSheet.Cell(CurrentRow, 16).Value = item.Parent?.FatherFamilyNameEnglish;
                    workSheet.Cell(CurrentRow, 17).Value = item.Parent?.FatherFirstNameArabic;
                    workSheet.Cell(CurrentRow, 18).Value = item.Parent?.FatherMiddleNameArabic;
                    workSheet.Cell(CurrentRow, 19).Value = item.Parent?.FatherFamilyNameArabic;
                    workSheet.Cell(CurrentRow, 20).Value = item.Parent?.FatherQualification;
                    workSheet.Cell(CurrentRow, 21).Value = item.Parent?.FatherOccupation;
                    workSheet.Cell(CurrentRow, 22).Value = item.Parent?.FatherPlaceOfWork;
                    workSheet.Cell(CurrentRow, 23).Value = item.Parent?.FatherWorkNumber;
                    workSheet.Cell(CurrentRow, 24).Value = item.Parent?.FatherMobileNumber;
                    workSheet.Cell(CurrentRow, 25).Value = item.Parent?.FatherEmailAddress;

                    workSheet.Cell(CurrentRow, 26).Value = item.Parent?.Mother_IdORIqama;
                    workSheet.Cell(CurrentRow, 27).Value = MotherNationality.ToString();
                    workSheet.Cell(CurrentRow, 28).Value = MotherReligion.ToString();
                    workSheet.Cell(CurrentRow, 29).Value = item.Parent?.MotherFirstNameEnglish;
                    workSheet.Cell(CurrentRow, 30).Value = item.Parent?.MotherMiddleNameEnglish;
                    workSheet.Cell(CurrentRow, 31).Value = item.Parent?.MotherFamilyNameEnglish;
                    workSheet.Cell(CurrentRow, 32).Value = item.Parent?.MotherQualification;
                    workSheet.Cell(CurrentRow, 33).Value = item.Parent?.MotherOccupation;
                    workSheet.Cell(CurrentRow, 34).Value = item.Parent?.MotherPlaceOfWork;
                    workSheet.Cell(CurrentRow, 35).Value = item.Parent?.MotherWorkNumber;
                    workSheet.Cell(CurrentRow, 36).Value = item.Parent?.MotherMobileNumber;
                    workSheet.Cell(CurrentRow, 37).Value = item.Parent?.MotherEmailAddress;


                    workSheet.Cell(CurrentRow, 38).Value = SufferFromPreviousFactor + " " + item.SufferFromPreviousFactorValue;
                    workSheet.Cell(CurrentRow, 39).Value = SepecialEducation + " " + item.SepecialEducationValue;
                    workSheet.Cell(CurrentRow, 40).Value = SkipeedRepeatedGrade + " " + item.SkipeedRepeatedGradeValue;
                    workSheet.Cell(CurrentRow, 41).Value = RepecialNeed + " " + item.RepecialNeedValue;
                    workSheet.Cell(CurrentRow, 42).Value = ReceivedAnyAward + " " + item.ReceivedAnyAwardValue;

                    workSheet.Cell(CurrentRow, 43).Value = TuitionPaymentMethods == 0 ? "" : TuitionPaymentMethods.GetAttribute<DisplayAttribute>().Name;
                    workSheet.Cell(CurrentRow, 44).Value = item.TuitionPlan?.TuitionPaymentPlan;

                    workSheet.Cell(CurrentRow, 45).Value = item.EmergencyContract1FullName;
                    workSheet.Cell(CurrentRow, 46).Value = item.EmergencyContract1RelationShip;
                    workSheet.Cell(CurrentRow, 47).Value = item.EmergencyContract1RelationPhoneNumber;
                    workSheet.Cell(CurrentRow, 48).Value = item.EmergencyContract2FullName;
                    workSheet.Cell(CurrentRow, 49).Value = item.EmergencyContract2RelationShip;
                    workSheet.Cell(CurrentRow, 50).Value = item.EmergencyContract2RelationPhoneNumber;

                    workSheet.Cell(CurrentRow, 51).Value = item.BirthCertificatePath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.BirthCertificatePath;
                    workSheet.Cell(CurrentRow, 52).Value = item.FamilyNationIDorFatherIqamaFronPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.FamilyNationIDorFatherIqamaFronPath;
                    workSheet.Cell(CurrentRow, 53).Value = item.FamilyNationIDorMotherIqamabackPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.FamilyNationIDorMotherIqamabackPath;
                    workSheet.Cell(CurrentRow, 55).Value = item.StudentImmunizationRecordPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.StudentImmunizationRecordPath;
                    workSheet.Cell(CurrentRow, 55).Value = item.StudentPassportPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.StudentPassportPath;
                    workSheet.Cell(CurrentRow, 56).Value = item.StudentMostGradeTranscriptPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.StudentMostGradeTranscriptPath;
                    workSheet.Cell(CurrentRow, 57).Value = item.StudentmedicalClearanceCertificatePath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.StudentmedicalClearanceCertificatePath;
                    workSheet.Cell(CurrentRow, 58).Value = item.Student64PhotoPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.Student64PhotoPath;
                    workSheet.Cell(CurrentRow, 59).Value = item.RuyaschoolAdministrationalFeesPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.RuyaschoolAdministrationalFeesPath;
                    workSheet.Cell(CurrentRow, 60).Value = item.FahterPassportPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.FahterPassportPath;
                    workSheet.Cell(CurrentRow, 61).Value = item.MotherPassportPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.MotherPassportPath;
                    workSheet.Cell(CurrentRow, 62).Value = item.StudentIqamaPath == null ? "" : "https://applicant.ruya.sch.sa//assets/Documents/" + item.StudentIqamaPath;

                }

                using (var stream = new MemoryStream())
                {
                    Workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Applicant.xlsx");
                }

            }
        }

        private bool EmailContent(string Email)
        {
            bool result = _emailSender.SendEmailAsync(
                   Email,
                    "Incompleted Applicant",
                     $"<p>Dear Respected Parents,</p>" +
                     $"<p>Greetings, and welcome to Ruya schools!</p>" +
                     $"<p>As we begin the enrolment process for the academic school year 2023-2024, we appreciate your immense trust in us by taking the decision to send your child(ren) to Ruya School. <br/> This is a notice that you have not yet completed the registration process and you have missed a few mandatory fields.</p>" +
                     $"<p>To complete your application, please log into your account at your earliest convenience.(<a href='https://applicant.ruya.sch.sa/Identity/Account/Login' target='_blank'>https://applicant.ruya.sch.sa/Identity/Account/Login</a>)</p>" +
                     $"<p>Please note that registration closes when classes reach capacity and incomplete applications will not be considered.</p>" +
                     $"<p>Your cooperation is greatly appreciated. If you have questions, please contact the school at the phone number.</p>" +
                     $"<p>Sincerely</p>");
            return result;
        }

        [Route("View/Policies")]
        public IActionResult Policies(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            EnrollmentUpdateModel RLUpdateModel = _mapper.Map<Applicant, EnrollmentUpdateModel>(applicant);
            ViewBag.RuyaDocuments = _context.RuyaDocuments.ToList();
            ViewBag.Id = Id;
            return View("View/Policies", RLUpdateModel);
        }
        [Route("View/Student_Info")]
        public IActionResult Student_Info(Guid Id)
        {
            Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }

            StudentInfoUpdateModel studentInfoUpdateModel = _mapper.Map<Applicant, StudentInfoUpdateModel>(applicant);
            ViewBag.Id = Id;
            return View("View/Student_Info", studentInfoUpdateModel);
        }
        [Route("View/Father_Info")]
        public IActionResult Father_Info(Guid Id)
        {

            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            Parent parent = applicant.Parent;

            FatherInfoUpdateModel fatherInfoUpdateModel = _mapper.Map<Parent, FatherInfoUpdateModel>(parent);
            ViewBag.Id = Id;
            return View("View/Father_Info", fatherInfoUpdateModel);
        }
        [Route("View/Mother_Info")]
        public IActionResult Mother_Info(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            Parent parent = applicant.Parent;

            MotherInfoUpdateModel motherInfoUpdateModel = _mapper.Map<Parent, MotherInfoUpdateModel>(parent);
            ViewBag.Id = Id;
            return View("View/Mother_Info", motherInfoUpdateModel);
        }
        [Route("View/Academic_Info")]
        public IActionResult Academic_Info(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            AcademicInfoUpdateModel academicInfoUpdateModel = _mapper.Map<Applicant, AcademicInfoUpdateModel>(applicant);
            ViewBag.Id = Id;
            return View("View/Academic_Info", academicInfoUpdateModel);
        }
        [Route("View/Tuition_Information")]
        public IActionResult Tuition_Information(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            List<TuitionPlan> tuitionPlan = _context.TuitionPlans.OrderBy(x => x.TuitionPaymentPlan).ToList();
            TuitionInfoUpdateModel tuitionInfoUpdateModel = _mapper.Map<Applicant, TuitionInfoUpdateModel>(applicant);
            ViewBag.TuitionPlan = tuitionPlan;
            ViewBag.Id = Id;
            return View("View/Tuition_Information", tuitionInfoUpdateModel);
        }
        [Route("View/Emergency_Info")]
        public IActionResult Emergency_Info(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            EmergencyInfoUpdateModel emergencyInfoUpdateModel = _mapper.Map<Applicant, EmergencyInfoUpdateModel>(applicant);
            ViewBag.Id = Id;
            return View("View/Emergency_Info", emergencyInfoUpdateModel);
        }
        [Route("View/Documents")]
        public IActionResult Documents(Guid Id)
        {
            Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
            if (applicant == null)
            {
                return NotFound();
            }
            DocumentUpdateModel documentUpdateModel = _mapper.Map<Applicant, DocumentUpdateModel>(applicant);
            ViewBag.Id = Id;
            return View("View/Documents", documentUpdateModel);
        }


        [Route("Edit/Policies")]
        public IActionResult PoliciesE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            EnrollmentUpdateModel RLUpdateModel = _mapper.Map<Applicant, EnrollmentUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            ViewBag.RuyaDocuments = _context.RuyaDocuments.ToList();
            return View("Edit/Policies", RLUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Policies")]
        public IActionResult Policies(EnrollmentUpdateModel model, string dir, string locationTo)
        {
            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "next")
                return RedirectToAction("Student_InfoE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "PoliciesE" });
            else
                return View(model);
        }


        [Route("Edit/Student_Info")]
        public IActionResult Student_InfoE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            StudentInfoUpdateModel studentInfoUpdateModel = _mapper.Map<Applicant, StudentInfoUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Student_Info", studentInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Student_Info")]
        public IActionResult Student_Info(StudentInfoUpdateModel model, string dir, string locationTo)
        {
            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "next")
                return RedirectToAction("Father_InfoE", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("PoliciesE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Student_InfoE" });
            else
                return View(model);
        }


        [Route("Edit/Father_Info")]
        public async Task<IActionResult> Father_InfoE(Guid Id)
        {
            Parent parent = null;
            if (Id == Guid.Empty || Id == null)
            {
                parent = HttpContext.Session.GetObject<Parent>("Parent");
                if (parent == null)
                {
                    parent = new Parent();
                }
                var userId = await _user.GetUserAsync(HttpContext.User);
                Parent FatherDataDB = _context.Parents.FirstOrDefault(x => x.UserId == userId.Id && x.submited == true);

                if (FatherDataDB != null && parent.Fathermapped == false)
                {
                    parent = FatherDataDB;
                    parent.Fathermapped = true;
                }
            }
            else
            {
                Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Parent parentCached = HttpContext.Session.GetObject<Parent>("Parent");
                    if (parentCached != null)
                    {
                        if (parentCached.Fathermapped == false)
                        {
                            parentCached = applicant.Parent;
                            parentCached.Fathermapped = true;
                            HttpContext.Session.SetObject("Parent", parentCached);
                        }
                        parent = parentCached;
                        parent.submited = applicant.Submited;
                    }
                }
            }



            FatherInfoUpdateModel fatherInfoUpdateModel = _mapper.Map<Parent, FatherInfoUpdateModel>(parent);
            ViewBag.Id = Id;
            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Father_Info", fatherInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Father_Info")]
        public IActionResult Father_Info(FatherInfoUpdateModel model, string dir, string locationTo)
        {
            Parent parent = getParent(model.ApplicantId);
            _mapper.Map(model, parent);
            HttpContext.Session.SetObject("Parent", parent);
            Applicant applicant = getApplicant(model.ApplicantId);
            HttpContext.Session.SetObject("Applicant", applicant);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.ApplicantId });
            }
            if (dir == "next")
                return RedirectToAction("Mother_InfoE", new { Id = model.ApplicantId });
            else if (dir == "previous")
                return RedirectToAction("Student_InfoE", new { Id = model.ApplicantId });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Father_InfoE" });
            else
                return View(model);
        }

        [Route("Edit/Mother_Info")]
        public async Task<IActionResult> Mother_InfoE(Guid Id)
        {
            Parent parent = null;
            if (Id == Guid.Empty || Id == null)
            {
                parent = HttpContext.Session.GetObject<Parent>("Parent");
                if (parent == null)
                {
                    parent = new Parent();
                }
                var userId = await _user.GetUserAsync(HttpContext.User);
                Parent FatherDataDB = _context.Parents.FirstOrDefault(x => x.UserId == userId.Id && x.submited == true);

                if (FatherDataDB != null && parent.Fathermapped == false)
                {
                    parent = FatherDataDB;
                    parent.Fathermapped = true;
                }
            }
            else
            {
                Applicant applicant = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Parent parentCached = HttpContext.Session.GetObject<Parent>("Parent");
                    if (parentCached != null)
                    {
                        if (parentCached.Fathermapped == false && applicant.Parent.submited == true)
                        {
                            parentCached = applicant.Parent;
                            parentCached.Fathermapped = true;
                            HttpContext.Session.SetObject("Parent", parentCached);
                        }
                        parent = parentCached;
                        parent.submited = applicant.Submited;
                    }
                }
            }

            MotherInfoUpdateModel motherInfoUpdateModel = _mapper.Map<Parent, MotherInfoUpdateModel>(parent);
            ViewBag.Id = Id;
            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Mother_Info", motherInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Mother_Info")]
        public IActionResult Mother_Info(MotherInfoUpdateModel model, string dir, string locationTo)
        {
            Parent parent = getParent(model.ApplicantId);
            _mapper.Map(model, parent);
            HttpContext.Session.SetObject("Parent", parent);
            Applicant applicant = getApplicant(model.ApplicantId);
            HttpContext.Session.SetObject("Applicant", applicant);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.ApplicantId });
            }
            if (dir == "next")
                return RedirectToAction("Academic_InfoE", new { Id = model.ApplicantId });
            else if (dir == "previous")
                return RedirectToAction("Father_InfoE", new { Id = model.ApplicantId });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Mother_InfoE" });
            else
                return View(model);
        }

        [Route("Edit/Academic_Info")]
        public IActionResult Academic_InfoE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            AcademicInfoUpdateModel academicInfoUpdateModel = _mapper.Map<Applicant, AcademicInfoUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Academic_Info", academicInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Academic_Info")]
        public IActionResult Academic_Info(AcademicInfoUpdateModel model, string dir, string locationTo)
        {

            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "next")
                return RedirectToAction("Tuition_InformationE", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Mother_InfoE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Academic_InfoE" });
            else
                return View(model);
        }

        [Route("Edit/Tuition_Information")]
        public IActionResult Tuition_InformationE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            List<TuitionPlan> tuitionPlan = _context.TuitionPlans.OrderBy(x => x.TuitionPaymentPlan).ToList();
            TuitionInfoUpdateModel tuitionInfoUpdateModel = _mapper.Map<Applicant, TuitionInfoUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            ViewBag.TuitionPlan = tuitionPlan;
            HttpContext.Session.Remove("Errors");
            return View("Edit/Tuition_Information", tuitionInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Tuition_Information")]
        public IActionResult Tuition_Information(TuitionInfoUpdateModel model, string dir, string locationTo)
        {
            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "next")
                return RedirectToAction("Emergency_InfoE", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Academic_InfoE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Tuition_InformationE" });
            else
                return View(model);
        }

        [Route("Edit/Emergency_Info")]
        public IActionResult Emergency_InfoE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            EmergencyInfoUpdateModel emergencyInfoUpdateModel = _mapper.Map<Applicant, EmergencyInfoUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Emergency_Info", emergencyInfoUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Emergency_Info")]
        public IActionResult Emergency_Info(EmergencyInfoUpdateModel model, string dir, string locationTo)
        {
            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "next")
                return RedirectToAction("DocumentsE", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Tuition_InformationE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Emergency_InfoE" });
            else
                return View(model);
        }

        [Route("Edit/Documents")]
        public IActionResult DocumentsE(Guid Id)
        {
            Applicant applicant = null;
            if (Id == Guid.Empty || Id == null)
            {
                applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            }
            else
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    return NotFound();
                }
                else
                {
                    Applicant applicantCashed = HttpContext.Session.GetObject<Applicant>("Applicant");
                    if (applicantCashed != null && applicant.Id == applicantCashed.Id)
                    {
                        applicantCashed.Submited = applicant.Submited;
                        applicant = applicantCashed;
                    }
                }
            }
            DocumentUpdateModel documentUpdateModel = _mapper.Map<Applicant, DocumentUpdateModel>(applicant);

            ViewBag.Errors = HttpContext.Session.GetObject<List<ErrorModel>>("Errors");
            HttpContext.Session.Remove("Errors");
            return View("Edit/Documents", documentUpdateModel);
        }

        [HttpPost]
        [Route("Edit/Documents")]
        public async Task<IActionResult> Documents(DocumentUpdateModel model, string dir, string locationTo)
        {
            if (model.BirthCertificatePathFile != null)
            {
                model.BirthCertificatePath = await uploadFile(model.BirthCertificatePathFile);
            }
            if (model.FamilyNationIDorFatherIqamaFronPathFile != null)
            {
                model.FamilyNationIDorFatherIqamaFronPath = await uploadFile(model.FamilyNationIDorFatherIqamaFronPathFile);
            }
            if (model.FamilyNationIDorMotherIqamabackPathFile != null)
            {
                model.FamilyNationIDorMotherIqamabackPath = await uploadFile(model.FamilyNationIDorMotherIqamabackPathFile);
            }
            if (model.StudentImmunizationRecordPathFile != null)
            {
                model.StudentImmunizationRecordPath = await uploadFile(model.StudentImmunizationRecordPathFile);
            }
            if (model.StudentPassportPathFile != null)
            {
                model.StudentPassportPath = await uploadFile(model.StudentPassportPathFile);
            }
            if (model.StudentMostGradeTranscriptPathFile != null)
            {
                model.StudentMostGradeTranscriptPath = await uploadFile(model.StudentMostGradeTranscriptPathFile);
            }
            if (model.StudentmedicalClearanceCertificatePathFile != null)
            {
                model.StudentmedicalClearanceCertificatePath = await uploadFile(model.StudentmedicalClearanceCertificatePathFile);
            }
            if (model.Student64PhotoPathFile != null)
            {
                model.Student64PhotoPath = await uploadFile(model.Student64PhotoPathFile);
            }
            if (model.RuyaschoolAdministrationalFeesPathFile != null)
            {
                model.RuyaschoolAdministrationalFeesPath = await uploadFile(model.RuyaschoolAdministrationalFeesPathFile);
            }
            if (model.FahterPassportPathFile != null)
            {
                model.FahterPassportPath = await uploadFile(model.FahterPassportPathFile);
            }
            if (model.MotherPassportPathFile != null)
            {
                model.MotherPassportPath = await uploadFile(model.MotherPassportPathFile);
            }
            if (model.StudentIqamaPathFile != null)
            {
                model.StudentIqamaPath = await uploadFile(model.StudentIqamaPathFile);
            }

            Applicant applicant = getApplicant(model.Id);
            _mapper.Map(model, applicant);
            HttpContext.Session.SetObject("Applicant", applicant);
            Parent parent = getParent(model.Id);
            HttpContext.Session.SetObject("Parent", parent);
            if (locationTo != null)
            {
                return RedirectToAction(locationTo, new { Id = model.Id });
            }
            if (dir == "previous")
                return RedirectToAction("Emergency_InfoE", new { Id = model.Id });
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "DocumentsE" });
            else
                return View(model);
        }

        public async Task<IActionResult> Save(string from)
        {
            try
            {
                Applicant applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
                Parent parent = HttpContext.Session.GetObject<Parent>("Parent");

                List<ErrorModel> errors = ValidateApplicant(applicant, parent);
                if (errors.Count() > 0)
                {
                    HttpContext.Session.SetObject("Errors", errors);
                    return RedirectToAction(from, new { Id = applicant.Id });
                }

                parent.submited = true;
                _context.Update(parent);
                await _context.SaveChangesAsync();

                applicant.Submited = true;
                applicant.ParentId = parent.Id;
                _context.Update(applicant);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Applicant");
                HttpContext.Session.Remove("Parent");
                return RedirectToAction("Completed_Applicants");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private List<ErrorModel> ValidateApplicant(Applicant applicant, Parent parent)
        {
            List<ErrorModel> errorModels = new List<ErrorModel>();
            ErrorModel errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (applicant.enrollment == null || applicant.enrollment == false)
            {
                errorModel.Error.Add("You Have to agree on Ruya school policies");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Policies Agreement";
                errorModels.Add(errorModel);
            }

            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();

            if (string.IsNullOrEmpty(applicant.Student_IdORIqama))
            {
                errorModel.Error.Add("(Student’s ID number or Iqama number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(applicant.Student_IdORIqama) && applicant.Student_IdORIqama.Length != 10)
            {
                errorModel.Error.Add("(Student’s ID number or Iqama number) Must be 10 number");
            }
            else if (!string.IsNullOrEmpty(applicant.Student_IdORIqama)
              && !Regex.Match(applicant.Student_IdORIqama, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Student’ ID number or Iqama number) Must be Number only no negative value");
            }

            if (string.IsNullOrEmpty(applicant.StudentFirstNameEnglish))
            {
                errorModel.Error.Add("(Student’s first name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(applicant.StudentFirstNameEnglish) &&
                !Regex.Match(applicant.StudentFirstNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Student’s first name in English) Only allow English Letter");
            }
            if (string.IsNullOrEmpty(applicant.StudentFirstNameArabic))
            {
                errorModel.Error.Add("(Student’s first name in Arabic) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(applicant.StudentFirstNameArabic) &&
                !Regex.Match(applicant.StudentFirstNameArabic, @"^[\u0621-\u064A\s]*$").Success)
            {
                errorModel.Error.Add("(Student’s first name in Arabic) Only allow Arabic Letter");
            }
            if (applicant.StudentBirthDate == null)
            {
                errorModel.Error.Add("(Student’s date of birth) Filed can't be Empty");
            }
            if (applicant.StudentCurrentLevel == null || applicant.StudentCurrentLevel == 0)
            {
                errorModel.Error.Add("(Student’s current level) Filed can't be Empty");
            }
            if (applicant.StudentUpcomingSchoolLevel == null || applicant.StudentUpcomingSchoolLevel == 0)
            {
                errorModel.Error.Add("(Upcoming school year) Filed can't be Empty");
            }
            //if (string.IsNullOrEmpty(applicant.StudentFirstLanguage))
            //{
            //    errorModel.Error.Add("(Student’s first language) Filed can't be Empty");
            //}
            //if (string.IsNullOrEmpty(applicant.StudentCurrentSchool))
            //{
            //    errorModel.Error.Add("Student’s current school) Filed can't be Empty");
            //}
            if (applicant.SchoolSystemCurrentlyForStudent == null || applicant.SchoolSystemCurrentlyForStudent == 0)
            {
                errorModel.Error.Add("(School system currently attended by the student:) Filed can't be Empty");
            }
            if (applicant.HasSiblingsAtRuya == null || applicant.HasSiblingsAtRuya == 0)
            {
                errorModel.Error.Add("(Does the student have siblings at Ruya School ?) Filed can't be Empty");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Student’s Details";
                errorModels.Add(errorModel);
            }

            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (string.IsNullOrEmpty(parent.Father_IdORIqama))
            {
                errorModel.Error.Add("(Father’s ID number or Iqama number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.Father_IdORIqama)
                && parent.Father_IdORIqama.Length != 10)
            {
                errorModel.Error.Add("(Father’s ID number or Iqama number) Must be 10 number");
            }
            else if (!string.IsNullOrEmpty(parent.Father_IdORIqama)
                && !Regex.Match(parent.Father_IdORIqama, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Father’s ID number or Iqama number) Must be Number only no negative value");
            }

            if (parent.FatherNationality == null || parent.FatherNationality == 0)
            {
                errorModel.Error.Add("(Father’s Nationality) Filed can't be Empty");
            }
            if (parent.FatherRegion == null || parent.FatherRegion == 0)
            {
                errorModel.Error.Add("(Father’s religion) Filed can't be Empty");
            }

            if (string.IsNullOrEmpty(parent.FatherFirstNameEnglish))
            {
                errorModel.Error.Add("(Father’s first name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherFirstNameEnglish) &&
                !Regex.Match(parent.FatherFirstNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s first name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherMiddleNameEnglish))
            {
                errorModel.Error.Add("(Father’s Middle name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherMiddleNameEnglish) &&
                !Regex.Match(parent.FatherMiddleNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s Middle name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherFamilyNameEnglish))
            {
                errorModel.Error.Add("(Father’s family name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherFamilyNameEnglish) &&
                !Regex.Match(parent.FatherFamilyNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s family name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherFirstNameArabic))
            {
                errorModel.Error.Add("(Father’s first name in Arabic) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherFirstNameArabic) &&
               !Regex.Match(parent.FatherFirstNameArabic, @"^[\u0621-\u064A\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s first name in Arabic) Only allow Arabic Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherMiddleNameArabic))
            {
                errorModel.Error.Add("(Father’s Middle name in Arabic) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherMiddleNameArabic) &&
               !Regex.Match(parent.FatherMiddleNameArabic, @"^[\u0621-\u064A\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s Middle name in Arabic) Only allow Arabic Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherFamilyNameArabic))
            {
                errorModel.Error.Add("(Father’s Family name in Arabic) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherFamilyNameArabic) &&
               !Regex.Match(parent.FatherFamilyNameArabic, @"^[\u0621-\u064A\s]*$").Success)
            {
                errorModel.Error.Add("(Father’s Family name in Arabic) Only allow Arabic Letter");
            }

            if (string.IsNullOrEmpty(parent.FatherQualification))
            {
                errorModel.Error.Add("(Father’s qualification) Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(parent.FatherOccupation))
            {
                errorModel.Error.Add("(Father’s occupation) Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(parent.FatherPlaceOfWork))
            {
                errorModel.Error.Add("(Father’s place of work) Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(parent.FatherWorkNumber))
            {
                errorModel.Error.Add("(Father’s work number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherWorkNumber)
               && parent.FatherWorkNumber.Length != 9)
            {
                errorModel.Error.Add("(Father’s work number) Must be 9 number");
            }
            else if (!string.IsNullOrEmpty(parent.FatherWorkNumber)
                && !Regex.Match(parent.FatherWorkNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Father’s work number) Must be Number only no negative value");
            }

            if (string.IsNullOrEmpty(parent.FatherMobileNumber))
            {
                errorModel.Error.Add("(Father’s mobile number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherMobileNumber)
                && parent.FatherMobileNumber.Length != 9)
            {
                errorModel.Error.Add("(Father’s mobile number) Must be 9 number");
            }
            else if (!string.IsNullOrEmpty(parent.FatherMobileNumber)
                && !Regex.Match(parent.FatherMobileNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Father’s mobile number) Must be Number only no negative value");
            }

            if (string.IsNullOrEmpty(parent.FatherEmailAddress))
            {
                errorModel.Error.Add("(Father’s email address) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.FatherEmailAddress) &&
                !Regex.Match(parent.FatherEmailAddress, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").Success)
            {
                errorModel.Error.Add("(Father’s email address) have to be in Email Adress Formate");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Father Information";
                errorModels.Add(errorModel);
            }


            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (string.IsNullOrEmpty(parent.Mother_IdORIqama))
            {
                errorModel.Error.Add("(Mother’s ID number or Iqama number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.Mother_IdORIqama) && parent.Mother_IdORIqama.Length != 10)
            {
                errorModel.Error.Add("(Mother’s ID number or Iqama number) Must be 10 number");
            }
            else if (!string.IsNullOrEmpty(parent.Mother_IdORIqama)
                && !Regex.Match(parent.Mother_IdORIqama, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Father’s ID number or Iqama number) Must be Number only no negative value");
            }

            if (parent.MotherNationality == null || parent.MotherNationality == 0)
            {
                errorModel.Error.Add("(Mother’s Nationality) Filed can't be Empty");
            }
            if (parent.MotherRegion == null || parent.MotherRegion == 0)
            {
                errorModel.Error.Add("(Mother’s religion) Filed can't be Empty");
            }

            if (string.IsNullOrEmpty(parent.MotherFirstNameEnglish))
            {
                errorModel.Error.Add("(Mother’s first name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.MotherFirstNameEnglish) &&
                !Regex.Match(parent.MotherFirstNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Mother’s first name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.MotherMiddleNameEnglish))
            {
                errorModel.Error.Add("(Mother’s Middle name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.MotherMiddleNameEnglish) &&
                !Regex.Match(parent.MotherMiddleNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Mother’s Middle name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.MotherFamilyNameEnglish))
            {
                errorModel.Error.Add("(Mother’s family name in English) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.MotherFamilyNameEnglish) &&
                !Regex.Match(parent.MotherFamilyNameEnglish, @"^[A-Za-z\s]*$").Success)
            {
                errorModel.Error.Add("(Mother’s family name in English) Only allow English Letter");
            }

            if (string.IsNullOrEmpty(parent.MotherQualification))
            {
                errorModel.Error.Add("(Mother’s qualification) Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(parent.MotherOccupation))
            {
                errorModel.Error.Add("(Mother’s occupation) Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(parent.MotherPlaceOfWork))
            {
                errorModel.Error.Add("(Mother’s place of work) Filed can't be Empty");
            }

            if (!string.IsNullOrEmpty(parent.MotherWorkNumber)
               && parent.MotherWorkNumber.Length != 9)
            {
                errorModel.Error.Add("(Mother’s work number) Must be 9 number");
            }
            else if (!string.IsNullOrEmpty(parent.MotherWorkNumber)
                && !Regex.Match(parent.MotherWorkNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Mother’s work number) Must be Number only no negative value");
            }

            if (string.IsNullOrEmpty(parent.MotherMobileNumber))
            {
                errorModel.Error.Add("(Mother’s work number) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.MotherMobileNumber)
                && parent.MotherMobileNumber.Length != 9)
            {
                errorModel.Error.Add("(Mother’s mobile number) Must be 10 number");
            }
            else if (!string.IsNullOrEmpty(parent.MotherMobileNumber)
                && !Regex.Match(parent.MotherMobileNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Mother’s mobile number) Must be Number only no negative value");
            }

            if (string.IsNullOrEmpty(parent.MotherEmailAddress))
            {
                errorModel.Error.Add("(Mother’s email address) Filed can't be Empty");
            }
            else if (!string.IsNullOrEmpty(parent.MotherEmailAddress) &&
                !Regex.Match(parent.MotherEmailAddress, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").Success)
            {
                errorModel.Error.Add("(Mother’s email address) have to be in Email Adress Formate");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Mother Information";
                errorModels.Add(errorModel);
            }

            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (applicant.SufferFromPreviousFactor == null || applicant.SufferFromPreviousFactor == 0)
            {
                errorModel.Error.Add("(Does the student suffer from any of the previous factors) Filed can't be Empty");
            }
            else if (applicant.SufferFromPreviousFactor == 2 && string.IsNullOrEmpty(applicant.SufferFromPreviousFactorValue))
            {
                errorModel.Error.Add("(Does the student suffer from any of the previous factors specify) can't be empty with Yes answer");
            }

            if (applicant.SepecialEducation == null || applicant.SepecialEducation == 0)
            {
                errorModel.Error.Add("(Has your child received services for learning disorder, special education, or previous program modification) Filed can't be Empty");
            }
            else if (applicant.SepecialEducation == 2 && string.IsNullOrEmpty(applicant.SepecialEducationValue))
            {
                errorModel.Error.Add("(Has your child received services for learning disorder, special education, or previous program modification specify) can't be empty with Yes answer");
            }

            if (applicant.SkipeedRepeatedGrade == null || applicant.SkipeedRepeatedGrade == 0)
            {
                errorModel.Error.Add("(Has the applicant skipped or repeated a grade /year? IF yes. which grade/year?) Filed can't be Empty");
            }
            else if (applicant.SkipeedRepeatedGrade == 2 && string.IsNullOrEmpty(applicant.SkipeedRepeatedGradeValue))
            {
                errorModel.Error.Add("(Has the applicant skipped or repeated a grade /year? IF yes. which grade/year? Specify) can't be empty with Yes answer");
            }

            if (applicant.RepecialNeed == null || applicant.RepecialNeed == 0)
            {
                errorModel.Error.Add("(Are you aware of any special needs (physical, social or academic) your child might have?) Filed can't be Empty");
            }
            else if (applicant.RepecialNeed == 2 && string.IsNullOrEmpty(applicant.RepecialNeedValue))
            {
                errorModel.Error.Add("(Are you aware of any special needs (physical, social or academic) your child might have? specify) can't be empty with Yes answer");

            }

            if (applicant.ReceivedAnyAward == null || applicant.ReceivedAnyAward == 0)
            {
                errorModel.Error.Add("(Has the applicant received any award ?) Filed can't be Empty");
            }
            else if (applicant.ReceivedAnyAward == 2 && string.IsNullOrEmpty(applicant.ReceivedAnyAwardValue))
            {
                errorModel.Error.Add("(Has the applicant received any award ? what ) can't be empty with Yes answer");

            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Academic Information";
                errorModels.Add(errorModel);
            }

            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (applicant.TuitionPaymentMethods == null || applicant.TuitionPaymentMethods == 0)
            {
                errorModel.Error.Add("Tuition payment method can't be empty");
            }
            if (applicant.TuitionPlanId == null && applicant.TuitionPlanId == Guid.Empty)
            {
                errorModel.Error.Add("Preferred Payment Plan method can't be empty");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Tuition Information";
                errorModels.Add(errorModel);
            }

            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (string.IsNullOrEmpty(applicant.EmergencyContract1FullName))
            {
                errorModel.Error.Add("(First Guardian Emergency Full Name) Can't be Empty");
            }
            if (string.IsNullOrEmpty(applicant.EmergencyContract1RelationShip))
            {
                errorModel.Error.Add("(First Guardian Emergency RelationShip) Can't be Empty");
            }
            if (string.IsNullOrEmpty(applicant.EmergencyContract1RelationPhoneNumber))
            {
                errorModel.Error.Add("(First Guardian Emergency Mobile Number) Can't be Empty");
            }
            else if (!string.IsNullOrEmpty(applicant.EmergencyContract1RelationPhoneNumber) && applicant.EmergencyContract1RelationPhoneNumber.Length != 9)
            {
                errorModel.Error.Add("(First Guardian Emergency Mobile Number) Must be 9 number");
            }
            else if (!string.IsNullOrEmpty(applicant.EmergencyContract1RelationPhoneNumber)
               && !Regex.Match(applicant.EmergencyContract1RelationPhoneNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(First Guardian Emergency Mobile Number) Number only no negative value");
            }

            if (string.IsNullOrEmpty(applicant.EmergencyContract2FullName))
            {
                errorModel.Error.Add("(Second Guardian Emergency Full Name) Can't be Empty");
            }
            if (string.IsNullOrEmpty(applicant.EmergencyContract2RelationShip))
            {
                errorModel.Error.Add("(Second Guardian Emergency RelationShip) Can't be Empty");
            }
            if (string.IsNullOrEmpty(applicant.EmergencyContract2RelationPhoneNumber))
            {
                errorModel.Error.Add("(Second Guardian Emergency Mobile Number) Can't be Empty");
            }
            else if (!string.IsNullOrEmpty(applicant.EmergencyContract2RelationPhoneNumber) && applicant.EmergencyContract2RelationPhoneNumber.Length != 9)
            {
                errorModel.Error.Add("(Second Guardian Emergency Mobile Number) Must be 9 number");
            }
            else if (!string.IsNullOrEmpty(applicant.EmergencyContract2RelationPhoneNumber)
               && !Regex.Match(applicant.EmergencyContract2RelationPhoneNumber, @"^[0-9]+$").Success)
            {
                errorModel.Error.Add("(Second Guardian Emergency Mobile Number) Must only no negative value");
            }

            if (errorModel.Error.Any())
            {
                errorModel.page = "Emergency Information";
                errorModels.Add(errorModel);
            }



            errorModel = new ErrorModel();
            errorModel.Error = new List<string>();
            if (string.IsNullOrEmpty(applicant.BirthCertificatePath))
            {
                errorModel.Error.Add("(Birth certificate)  have to upload document");
            }
            if (string.IsNullOrEmpty(applicant.FamilyNationIDorFatherIqamaFronPath))
            {
                errorModel.Error.Add("(Family national ID: Front side Or Fathers iqama id for non - saudis)  have to upload document");
            }
            if (string.IsNullOrEmpty(applicant.FamilyNationIDorMotherIqamabackPath))
            {
                errorModel.Error.Add("(Family national ID: Back sideOr Mother’s iqama id for non - saudis )  have to upload document");
            }
            if (string.IsNullOrEmpty(applicant.StudentImmunizationRecordPath))
            {
                errorModel.Error.Add("(Student’s immunization record)  have to upload document");
            }
            if (string.IsNullOrEmpty(applicant.StudentPassportPath))
            {
                errorModel.Error.Add("(Student’s passport)  have to upload document");
            }
            //if (string.IsNullOrEmpty(applicant.StudentmedicalClearanceCertificatePath))
            //{
            //    errorModel.Error.Add("(Medical Clearance certificate)  have to upload document");
            //}
            if (string.IsNullOrEmpty(applicant.Student64PhotoPath))
            {
                errorModel.Error.Add("(4*6 photograph)  Filed can't be Empty");
            }
            if (string.IsNullOrEmpty(applicant.RuyaschoolAdministrationalFeesPath))
            {
                errorModel.Error.Add("(Ruya school administrational fees receipt)  have to upload document");
            }
            if (applicant.DocusAuthentic == false)
            {
                errorModel.Error.Add("you have to confirm that all document you submitted are true and authentic");
            }
            if (errorModel.Error.Any())
            {
                errorModel.page = "Documents";
                errorModels.Add(errorModel);
            }
            return errorModels;
        }

        private async Task<string> uploadFile(IFormFile file)
        {
            var _fileDeafaultPath = ((PhysicalFileProvider)_fileProvider).Root;
            var empImagePath = Path.Combine(_fileDeafaultPath, "Documents\\");
            Directory.CreateDirectory(empImagePath);
            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string imgFullName = Path.Combine(empImagePath, newFileName);
            using (FileStream target = new FileStream(imgFullName, FileMode.Create))
            {
                await file.CopyToAsync(target);
            }
            return Path.GetFileName(newFileName);
        }

        private Applicant getApplicant(Guid? Id)
        {
            Applicant applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
            if (Id != null && Id != Guid.Empty && (applicant == null || applicant.Id != Id))
            {
                applicant = _context.Applicants.FirstOrDefault(x => x.Id == Id);
                if (applicant == null)
                {
                    applicant = new Applicant();
                }
            }
            if (applicant == null)
            {
                applicant = new Applicant();
            }
            return applicant;
        }

        private Parent getParent(Guid Id)
        {
            Parent parent = HttpContext.Session.GetObject<Parent>("Parent");
            if (parent == null)
            {
                var userId = _user.GetUserAsync(HttpContext.User);
                Parent parentDb = _context.Parents.FirstOrDefault(x => x.UserId == userId.Result.Id && x.submited == true);
                if (parentDb == null)
                {
                    var applicantParent = _context.Applicants.Include(x => x.Parent).Where(x => x.Id == Id).Select(x => x.Parent).FirstOrDefault();
                    if (applicantParent == null)
                    {
                        parent = new Parent();
                    }
                    else
                    {
                        parent = applicantParent;
                    }
                }
                else
                {
                    parent = parentDb;
                }
            }

            return parent;
        }

        public IActionResult SendEmail(Guid Id, string from)
        {
            ViewBag.Id = Id;
            ViewBag.From = from;
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(SendEmailModel model)
        {
            Parent parent = _context.Applicants.Include(x => x.Parent).FirstOrDefault(x => x.Id == model.Id).Parent;
            if (model.IncludeFather && !string.IsNullOrEmpty(parent.FatherEmailAddress))
            {
                sendCustomeEmail(parent.FatherEmailAddress, model.Title, model.Body);
            }
            if (model.IncludeMother && !string.IsNullOrEmpty(parent.MotherEmailAddress))
            {
                sendCustomeEmail(parent.MotherEmailAddress, model.Title, model.Body);
            }
            return Redirect(model.From);
        }

        private bool sendCustomeEmail(string Email, string Title, string body)
        {
            bool result = _emailSender.SendEmailAsync(Email, Title, body);
            return result;
        }

        private bool SendCustomeEmailAlternativeAsync(string Email, string Title, AlternateView body)
        {
            bool result = _emailSender.SendEmailAlternativeAsync(Email, Title, body);
            return result;
        }
    }
}
