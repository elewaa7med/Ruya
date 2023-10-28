using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Entities;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models.Academic_Info;
using SmartAdmin.WebUI.Models.Dashboard;
using SmartAdmin.WebUI.Models.Documents;
using SmartAdmin.WebUI.Models.EmergencyInfo;
using SmartAdmin.WebUI.Models.Enrollment;
using SmartAdmin.WebUI.Models.ErrorFormate;
using SmartAdmin.WebUI.Models.Father_Info;
using SmartAdmin.WebUI.Models.Mother_Info;
using SmartAdmin.WebUI.Models.Student_Info;
using SmartAdmin.WebUI.Models.Tuition_Info;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class Ruya : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<User> _user;
        private readonly SignInManager<User> _signInManager;


        public Ruya(ApplicationContext context, IMapper mapper, IFileProvider fileProvider,
            UserManager<User> user, SignInManager<User> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _fileProvider = fileProvider;
            _user = user;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
            try
            {
                var userId = await _user.GetUserAsync(HttpContext.User);
                if (userId == null)
                    return Redirect("Identity/Account/login");
                List<Applicant> applicants = _context.Applicants.Where(x => x.UserId == userId.Id && x.Deleted != true).ToList();
                HttpContext.Session.Remove("Applicant");
                HttpContext.Session.Remove("Parent");
                List<ApplicantDashboardViewModel> appliicantDashboardViewModels = _mapper.Map<List<Applicant>, List<ApplicantDashboardViewModel>>(applicants);
                return View(appliicantDashboardViewModels);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public IActionResult Policies(Guid? Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(RLUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Student_Info", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Policies" });
            else
                return View(model);
        }


        public IActionResult Student_Info(Guid? Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(studentInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Father_Info", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Policies", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Student_Info" });
            else
                return View(model);
        }


        public async Task<IActionResult> Father_Info(Guid? Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(fatherInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Mother_Info", new { Id = model.ApplicantId });
            else if (dir == "previous")
                return RedirectToAction("Student_Info", new { Id = model.ApplicantId });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Father_Info" });
            else
                return View(model);
        }

        public async Task<IActionResult> Mother_Info(Guid? Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(motherInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Academic_Info", new { Id = model.ApplicantId });
            else if (dir == "previous")
                return RedirectToAction("Father_Info", new { Id = model.ApplicantId });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Mother_Info" });
            else
                return View(model);
        }

        public IActionResult Academic_Info(Guid? Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(academicInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Tuition_Information", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Mother_Info", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Academic_Info" });
            else
                return View(model);
        }

        public IActionResult Tuition_Information(Guid Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(tuitionInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Emergency_Info", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Academic_Info", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Tuition_Information" });
            else
                return View(model);
        }

        public IActionResult Emergency_Info(Guid Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(emergencyInfoUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Documents", new { Id = model.Id });
            else if (dir == "previous")
                return RedirectToAction("Tuition_Information", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Emergency_Info" });
            else
                return View(model);
        }


        public IActionResult Documents(Guid Id)
        {
            bool admin = ValidateRole();
            if (admin) return Redirect("Admin/Dashboard");
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
            return View(documentUpdateModel);
        }

        [HttpPost]
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
                return RedirectToAction("Emergency_Info", new { Id = model.Id });
            else if (dir == "savelater")
                return RedirectToAction("SaveForLater");
            else if (dir == "submit")
                return RedirectToAction("Save", new { from = "Documents" });
            else
                return View(model);
        }

        public async Task<IActionResult> SaveForLater()
        {
            try
            {
                Applicant applicant = HttpContext.Session.GetObject<Applicant>("Applicant");
                Parent parent = HttpContext.Session.GetObject<Parent>("Parent");
                if (string.IsNullOrEmpty(applicant.UserId))
                {
                    var userId = await _user.GetUserAsync(HttpContext.User);
                    applicant.UserId = userId.Id;
                    var parentDB = _context.Parents.FirstOrDefault(x => x.UserId == userId.Id && x.submited == true);
                    if (parentDB == null)
                    {
                        parent.UserId = userId.Id;
                        await _context.AddAsync(parent);
                    }
                    else
                    {
                        parent.Id = parentDB.Id;
                        _context.Update(parent);
                    }
                    applicant.ParentId = parent.Id;
                    applicant.Status = 1;
                    await _context.AddAsync(applicant);
                }
                else
                {
                    var userId = await _user.GetUserAsync(HttpContext.User);
                    applicant.UserId = userId.Id;
                    var parentDB = _context.Parents.FirstOrDefault(x => x.UserId == userId.Id && x.submited == true);
                    if (parentDB == null)
                    {
                        var applicantParent = _context.Applicants.Include(x => x.Parent).Where(x => x.Id == applicant.Id).Select(x => x.Parent).FirstOrDefault();
                        if (applicantParent == null)
                        {
                            parent.UserId = userId.Id;
                            await _context.AddAsync(parent);
                        }
                        else
                        {

                            parent.Id = applicantParent.Id;
                            _context.Update(parent);
                        }
                    }
                    else
                    {
                        parent.Id = parentDB.Id;
                        parent.submited = true;
                        _context.Update(parent);
                    }
                    await _context.SaveChangesAsync();

                    applicant.ParentId = parent.Id;
                    _context.Applicants.Update(applicant);
                }
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Applicant");
                HttpContext.Session.Remove("Parent");
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                return View();
            }
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

                var userId = await _user.GetUserAsync(HttpContext.User);

                parent.UserId = userId.Id;
                parent.submited = true;
                _context.Update(parent);
                await _context.SaveChangesAsync();

                applicant.UserId = userId.Id;
                applicant.Submited = true;
                applicant.ParentId = parent.Id;
                if (applicant.Id != null && applicant.Id != Guid.Empty)
                {
                    _context.Update(applicant);
                }
                else
                {
                    applicant.Status = 1;
                    _context.Add(applicant);
                }
                await _context.SaveChangesAsync();

                var applicantsUpdate = _context.Applicants.Where(x => x.UserId == userId.Id && x.Submited != true).ToList();
                foreach (var applicantin in applicantsUpdate)
                {
                    applicantin.ParentId = parent.Id;
                }
                _context.UpdateRange(applicantsUpdate);
                await _context.SaveChangesAsync();


                var parents = _context.Parents.Where(x => x.UserId == userId.Id && x.submited != true).ToList();
                _context.RemoveRange(parents);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Applicant");
                HttpContext.Session.Remove("Parent");
                return RedirectToAction("Sucess");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public IActionResult Sucess()
        {
            return View();
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

        private bool ValidateRole()
        {
            var user = _user.GetUserAsync(HttpContext.User).Result;
            var isAdmin = _user.IsInRoleAsync(user, "admin").Result;
            return isAdmin;
        }

    }


}
