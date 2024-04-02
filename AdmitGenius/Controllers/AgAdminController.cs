using AdmitGenius.Models.ViewModels;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;
using AdmitGenius.Data;

namespace AdmitGenius.Controllers
{
    public class AgAdminController : Controller
    {
        private readonly AdmitDBContext adminDbContext;
        IWebHostEnvironment hostingenvironment;

        public AgAdminController(AdmitDBContext adminDbContext,IWebHostEnvironment hc)
        {
            this.adminDbContext = adminDbContext;
            hostingenvironment = hc;
        }


        //-------------------------------------------------------------------------//
        //Dashbaord

        public IActionResult AgDashboard()
        {
            return View();
        }


        //-------------------------------------------------------------------------//
        //add new university

        public IActionResult AddNewUniversity()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewUniversity(AddUniversityViewModel addUniversityRequest)
        {
            var universityId = Guid.NewGuid();
            String filename = "";

            if (addUniversityRequest.UniversityImageFile!=null)
            {
                String uploadfolder = Path.Combine(hostingenvironment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + addUniversityRequest.UniversityImageFile.FileName;

                String filepath=Path.Combine(uploadfolder,filename);
                addUniversityRequest.UniversityImageFile.CopyTo(new FileStream(filepath, FileMode.Create));

            }

            var universities = new Universities()
            {
                UniversityId = universityId,
                UniversityName = addUniversityRequest.UniversityName,
                UniversityDescription = addUniversityRequest.UniversityDescription,
                UniversityImage = filename,
                UniversityCountry = addUniversityRequest.UniversityCountry,
                UniversityLocation = addUniversityRequest.UniversityLocation,
                UniversityRegisterDate = addUniversityRequest.UniversityRegisterDate,
                UniversityOfficalEmailAddress = addUniversityRequest.UniversityOfficalEmailAddress,
                UniversityPassword = addUniversityRequest.UniversityPassword,
                RequestStatus = "Approved"
            };

            await adminDbContext.Universities.AddAsync(universities);
            await adminDbContext.SaveChangesAsync();
            ViewBag.sucess = "record added";
            await AddUserForUniversity(universityId, addUniversityRequest.UniversityOfficalEmailAddress, addUniversityRequest.UniversityPassword, addUniversityRequest.UniversityName, addUniversityRequest.UniversityName);

            return RedirectToAction("StudentHomePage","StudentHome");
        }

        private async Task AddUserForUniversity(Guid universityId, string userEmail, string userPassword, string userName,string UserProfile)
        {
            var user = new Users()
            {
                UserId = universityId,
                UserEmail = userEmail,
                UserName = userName,
                UserProfile = UserProfile,
                UserPassword = userPassword,
                UserRole = "University",
            };

            await adminDbContext.Users.AddAsync(user);
            await adminDbContext.SaveChangesAsync();
        }


        //-------------------------------------------------------------------------//
        // Add new counselor
        [HttpGet]
        public IActionResult AddNewCounselor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCounselor(AddCounselorViewModel addCounselorRequest)
        {
            var counselorId = Guid.NewGuid();

            var counselor = new Counselors()
            {
                CounselorId = counselorId,
                CounselorFirstName = addCounselorRequest.CounselorFirstName,
                CounselorEmail = addCounselorRequest.CounselorEmail,
                CounselorPassword = addCounselorRequest.CounselorPassword,
                CounselorPhone = addCounselorRequest.CounselorPhone,
                CounselorCountry = addCounselorRequest.CounselorCountry,
                CounselorFullLocation = addCounselorRequest.CounselorFullLocation,
                RequestStatus = "Approved"
            };

            await adminDbContext.Counselors.AddAsync(counselor);
            await adminDbContext.SaveChangesAsync();
            await AddUserForCounselor(counselorId, addCounselorRequest.CounselorEmail, addCounselorRequest.CounselorPassword, addCounselorRequest.CounselorFirstName, addCounselorRequest.CounselorFirstName);

            return RedirectToAction("AddNewCounselor");
        }

        private async Task AddUserForCounselor(Guid counselorId, string userEmail, string userPassword, string userName, string userProfile)
        {
            var user = new Users()
            {
                UserId = counselorId,
                UserEmail = userEmail,
                UserName = userName,
                UserProfile = userProfile,
                UserPassword = userPassword,
                UserRole = "Counselor"
            };

            await adminDbContext.Users.AddAsync(user);
            await adminDbContext.SaveChangesAsync();
        }


        //-------------------------------------------------------------------------//
        // Approve Universities

        [HttpGet]
        public IActionResult UniversityRegisterRequests()
        {
            // Retrieve universities with pending requests
            var universities = adminDbContext.Universities
                .Where(u => u.RequestStatus == "Pending")
                .ToList();

            return View(universities);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUniversityRequestStatus(Guid universityId, string newStatus)
        {
            try
            {
                var university = await adminDbContext.Universities.FindAsync(universityId);

                if (university == null || university.RequestStatus != "Pending")
                {
                    ViewBag.Error = "Invalid university or request status.";
                    return RedirectToAction("UniversityList");
                }

                // Set request status to the new status (Approved or Rejected)
                university.RequestStatus = newStatus;

                // Save changes to the database
                await adminDbContext.SaveChangesAsync();

                // Add a new user for the approved university
                await AddUserForUniversity(university.UniversityId, university.UniversityOfficalEmailAddress, university.UniversityPassword, university.UniversityName, university.UniversityImage);

                return RedirectToAction("UniversityRegisterRequests");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ViewBag.Error = "An error occurred while processing the university registration.";
                return RedirectToAction("UniversityList" + ex);
            }
        }



        //-------------------------------------------------------------------------//
        // Approve Counselors

        [HttpGet]
        public IActionResult CounselorRegisterRequests()
        {
            // Retrieve counselors with pending requests
            var counselors = adminDbContext.Counselors
                .Where(c => c.RequestStatus == "Pending")
                .ToList();

            return View(counselors);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCounselorRequestStatus(Guid counselorId, string newStatus)
        {
            try
            {
                var counselor = await adminDbContext.Counselors.FindAsync(counselorId);

                if (counselor == null || counselor.RequestStatus != "Pending")
                {
                    ViewBag.Error = "Invalid counselor or request status.";
                    return RedirectToAction("CounselorList");
                }

                // Set request status to the new status (Approved or Rejected)
                counselor.RequestStatus = newStatus;

                // Save changes to the database
                await adminDbContext.SaveChangesAsync();

                // Add a new user for the approved counselor
                await AddUserForCounselor(counselor.CounselorId, counselor.CounselorEmail, counselor.CounselorPassword, counselor.CounselorFirstName, counselor.CounselorProfilePicture);

                return RedirectToAction("CounselorRegisterRequests");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ViewBag.Error = "An error occurred while processing the counselor registration.";
                return RedirectToAction("CounselorList "+ex);
            }
        }



        //-------------------------------------------------------------------------//
        // Approve Company

        [HttpGet]
        public IActionResult CompanyRegisterRequests()
        {
            // Retrieve universities with pending requests
            var company = adminDbContext.Company
                .Where(u => u.RequestStatus == "Pending")
                .ToList();

            return View(company);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCompanyRequestStatus(Guid companyId, string newStatus)
        {
            try
            {
                var company = await adminDbContext.Company.FindAsync(companyId);

                if (company == null || company.RequestStatus != "Pending")
                {
                    ViewBag.Error = "Invalid university or request status.";
                    return RedirectToAction("UniversityList");
                }

                // Set request status to the new status (Approved or Rejected)
                company.RequestStatus = newStatus;

                // Save changes to the database
                await adminDbContext.SaveChangesAsync();

                // Add a new user for the approved university
                await AddUserForCompany(company.CompanyId, company.CompanyOfficalEmailAddress, company.CompanyPassword, company.CompanyName, company.CompanyImage);

                return RedirectToAction("UniversityRegisterRequests");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ViewBag.Error = "An error occurred while processing the university registration.";
                return RedirectToAction("UniversityList" + ex);
            }
        }

        private async Task AddUserForCompany(Guid CompanyId, string userEmail, string userPassword, string userName, string UserProfile)
        {
            var user = new Users()
            {
                UserId = CompanyId,
                UserEmail = userEmail,
                UserName = userName,
                UserProfile = UserProfile,
                UserPassword = userPassword,
                UserRole = "Company",
            };

            await adminDbContext.Users.AddAsync(user);
            await adminDbContext.SaveChangesAsync();
        }
    }
}