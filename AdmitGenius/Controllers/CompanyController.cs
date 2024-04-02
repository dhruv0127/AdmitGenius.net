using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdmitGenius.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AdmitDBContext dbContext;

        public CompanyController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }





        public IActionResult AllCompanies(string search, string location, string jobType)
        {
            var approvedCompanies = dbContext.Company
                .Where(u => u.RequestStatus == "Approved")
                .ToList();

            var jobs = dbContext.Job.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                // Apply case-insensitive search criteria to Company entity
                approvedCompanies = approvedCompanies
                    .Where(c =>
                        c.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CompanyDescription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CompanyTagline.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CompanyCountry.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CompanyLocation.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CompanyWebsite.Contains(search, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();

                // Apply case-insensitive search criteria to Job entity
                jobs = jobs
                    .Where(j =>
                        j.Company.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.Company.CompanyCountry.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.Company.CompanyLocation.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobDescription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobEmployeeType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobLocation.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobResponsibility.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        j.JobRequirements.Contains(search, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // Apply additional filters based on the selected values from dropdowns
            if (!string.IsNullOrEmpty(location))
            {
                approvedCompanies = approvedCompanies
                    .Where(c => c.CompanyCountry.Equals(location, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                jobs = jobs
                   .Where(j => j.JobLocation.Equals(location, StringComparison.OrdinalIgnoreCase))
                   .ToList();
            }

            if (!string.IsNullOrEmpty(jobType))
            {
                jobs = jobs
                    .Where(j => j.JobEmployeeType.Equals(jobType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var viewModel = new AllCompaniesViewModel
            {
                Company = approvedCompanies,
                Job = jobs
            };

            return View(viewModel);
        }





        public async Task<IActionResult> CompanyPage(Guid CompanyId)
        {
            var companyWithJpb = await dbContext.Company
                .Include(u => u.Job)
                .Include(u => u.Teams)
                .FirstOrDefaultAsync(u => u.CompanyId == CompanyId);

            if (companyWithJpb == null)
            {
                return NotFound();
            }

            return View(companyWithJpb);
        }


        public async Task<IActionResult> JobDescription(Guid JobId)
        {
            var job = await dbContext.Job
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.JobId == JobId);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }



        //handle enrollemtns



        [HttpPost]
        public async Task<IActionResult> Enroll(Guid JobId)
        {
            Guid studentId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            if (studentId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }


            var job = await dbContext.Job
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.JobId == JobId);


            var intern = new InternRequest
            {
                IrID = Guid.NewGuid(),
                DateOfRequest = DateTime.Now,
                Status = "pending",
                UserId = studentId,
                JobId = JobId,
                ccid = job.CompanyId
            };

            await dbContext.InternRequest.AddAsync(intern);
            await dbContext.SaveChangesAsync();

            ViewBag.CompanyName = job.Company.CompanyName;

            return RedirectToAction("EnrollmentSuccess");

        }

        public IActionResult EnrollmentSuccess()
        {
            return View(ViewBag.CompanyName);
        }

        [HttpGet]
        public IActionResult Dashboard()
        {



            Guid companyId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var TotalWorkers = dbContext.Teams.Where(t => t.CompanyId == companyId).Count();
            var TotalInterns = dbContext.Teams.Where(t => t.CompanyId == companyId && t.TeamPost.ToLower() == "intern").Count();
            var TotalEmployee = TotalInterns;

            var PendingEnrollments = dbContext.InternRequest.Where(ir => ir.ccid == companyId && ir.Status.ToLower() == "pending").Count();
            var ApprovedEnrollments = dbContext.InternRequest.Where(ir => ir.ccid == companyId && ir.Status.ToLower() == "approved").Count();
            var RejectedEnrollments = dbContext.InternRequest.Where(ir => ir.ccid == companyId && ir.Status.ToLower() == "rejected").Count();


            ViewBag.TotalEmployee = TotalEmployee;
            ViewBag.TotalInterns = TotalInterns;
            ViewBag.TotalWorkers = TotalWorkers;

            ViewBag.PendingEnrollments = PendingEnrollments;
            ViewBag.ApprovedEnrollments = ApprovedEnrollments;
            ViewBag.RejectedEnrollments = RejectedEnrollments;


            var jobApplications = dbContext.InternRequest
                                   .Include(ir => ir.Job)
                                   .Include(x => x.User)
                                   .Where(ir => ir.Job.CompanyId == companyId && ir.Status.ToLower() == "pending")

                                   .ToList();

            return View(jobApplications);
        }



        /* Add Teams */

        [HttpGet]
        public IActionResult AddNewTeam()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewTeam(AddTeamViewModel addTeamViewModel)
        {

            Guid Cid = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var teams = new Teams()
            {
                TeamId = Guid.NewGuid(),
                TeamName = addTeamViewModel.TeamName,
                TeamPost = addTeamViewModel.TeamPost,

                UniversityId = null,
                CompanyId = Cid
            };

            if (addTeamViewModel.TeamProfile != null)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        addTeamViewModel.TeamProfile.CopyTo(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = $"{addTeamViewModel.TeamProfile.FileName}",
                            BucketName = "yourbucketname",
                            ContentType = addTeamViewModel.TeamProfile.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);

                        teams.TeamProfile = $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            await dbContext.Teams.AddAsync(teams);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("AddNewTeam");
        }








        // Dashbaords

        public IActionResult AddJobPosting()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddJobPosting(AddJobViewModel addJobViewModel)
        {
            Guid sessionCompanyId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var job = new Job
            {
                JobId = Guid.NewGuid(),
                CompanyId = sessionCompanyId,
                JobName = addJobViewModel.JobName,
                JobDescription = addJobViewModel.JobDescription,
                JobPostedTime = DateTime.Now,
                JobDesignation=addJobViewModel.JobDesignation,
                JobEmployeeType = addJobViewModel.JobEmployeeType,
                JobLocation = addJobViewModel.JobLocation,
                JobMinSalary = addJobViewModel.JobMinSalary,
                JobMaxSalary = addJobViewModel.JobMaxSalary,

                JobMiniQualification = addJobViewModel.JobMiniQualification,

                JobRequirements = addJobViewModel.JobRequirements,
                JobResponsibility = addJobViewModel.JobResponsibility,

            };

            await dbContext.Job.AddAsync(job);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CompanyPage");
        }



        public async Task<IActionResult> EditCompanyDetails()
        {
            // Get the University ID from the session
            Guid sessionCompanyId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            // Retrieve the university details
            var Company = await dbContext.Company.FindAsync(sessionCompanyId);

            if (Company == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Map the university details to a view model
            var editCompanyViewModel = new AddComapnyViewModel
            {
                CompanyId = Company.CompanyId,
                CompanyName = Company.CompanyName,
                CompanyTagline = Company.CompanyTagline,
                CompanyDescription = Company.CompanyDescription,
                CompanyRegisterDate = Company.CompanyRegisterDate,

                CompanyImageLocation = Company.CompanyImage,
                CompanyLogoLocation = Company.CompanyLogo,


                CompanyCountry = Company.CompanyCountry,
                CompanyLocation = Company.CompanyLocation,

                TotalClients = Company.TotalClients,
                TotalProjects = Company.TotalProjects,
                TotalWorkers = Company.TotalWorkers,


                Feature1 = Company.Feature1,
                Feature2 = Company.Feature2,
                Feature3 = Company.Feature3,
                Feature4 = Company.Feature4,
                Feature5 = Company.Feature5,
                Feature6 = Company.Feature6,

                TitleOfFeature1 = Company.TitleOfFeature1,
                TitleOfFeature2 = Company.TitleOfFeature2,
                TitleOfFeature3 = Company.TitleOfFeature3,
                TitleOfFeature4 = Company.TitleOfFeature4,
                TitleOfFeature5 = Company.TitleOfFeature5,
                TitleOfFeature6 = Company.TitleOfFeature6,

                CompanyWebsite = Company.CompanyWebsite,


                logitude = Company.logitude,
                latitude = Company.latitude
            };

            return View(editCompanyViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditCompanyDetails(AddComapnyViewModel editViewModel)
        {

            Guid sessionCompanyId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            // Retrieve the university entity
            var Company = await dbContext.Company.FindAsync(sessionCompanyId);

            if (Company == null)
            {
                return RedirectToAction("Login", "Users");
            }



            //============image ================//  

            if (editViewModel.CompanyImage != null)
            {
                Company.CompanyImage = HandleFileUploads(editViewModel.CompanyImage);
            }

            if (editViewModel.CompanyLogo != null)
            {
                Company.CompanyLogo = HandleFileUploads(editViewModel.CompanyLogo);

            }

            // Update other details
            Company.CompanyName = editViewModel.CompanyName;
            Company.CompanyTagline = editViewModel.CompanyTagline;
            Company.CompanyDescription = editViewModel.CompanyDescription;
            Company.CompanyRegisterDate = editViewModel.CompanyRegisterDate;
            Company.CompanyCountry = editViewModel.CompanyCountry;
            Company.CompanyLocation = editViewModel.CompanyLocation;


            // Update location details
            Company.logitude = editViewModel.logitude;
            Company.latitude = editViewModel.latitude;


            Company.TotalClients = editViewModel.TotalClients;
            Company.TotalProjects = editViewModel.TotalProjects;
            Company.TotalWorkers = editViewModel.TotalWorkers;


            // Update features
            Company.Feature1 = editViewModel.Feature1;
            Company.Feature2 = editViewModel.Feature2;
            Company.Feature3 = editViewModel.Feature3;
            Company.Feature4 = editViewModel.Feature4;
            Company.Feature5 = editViewModel.Feature5;
            Company.Feature6 = editViewModel.Feature6;


            Company.TitleOfFeature1 = editViewModel.TitleOfFeature1;
            Company.TitleOfFeature2 = editViewModel.TitleOfFeature2;
            Company.TitleOfFeature3 = editViewModel.TitleOfFeature3;
            Company.TitleOfFeature4 = editViewModel.TitleOfFeature4;
            Company.TitleOfFeature5 = editViewModel.TitleOfFeature5;
            Company.TitleOfFeature6 = editViewModel.TitleOfFeature6;

            Company.CompanyWebsite = editViewModel.CompanyWebsite;

            dbContext.Entry(Company).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            // Redirect to the university dashboard or another appropriate page
            return RedirectToAction("EditCompanyDetails");
        }



        private string HandleFileUploads(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = imageFile.FileName,
                            BucketName = "yourbucketname",
                            ContentType = imageFile.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        transferUtility.Upload(request);

                        return $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            return null;
        }











        // manage enrollemtns 

        public async Task<IActionResult> ManageEnrollments()
        {
            Guid Cid = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var pendingEnrollments = await dbContext.InternRequest
               .Where(e => e.ccid == Cid && e.Status == "pending")
               .Include(e => e.Job)
               .Include(e => e.User)
               .ToListAsync();

            return View(pendingEnrollments);
        }


        [HttpPost]
        public async Task<IActionResult> AcceptEnrollment(Guid enrollmentId)
        {
            var enrollment = await dbContext.InternRequest.FindAsync(enrollmentId);
            var intern = await dbContext.Users.FindAsync(enrollment.UserId);
            var job = await dbContext.Job.Where(x => x.JobId == enrollment.JobId).FirstOrDefaultAsync();



            if (enrollment == null)
            {
                return NotFound();
            }

            // Update the status to "Approved"
            enrollment.Status = "Approved";
            dbContext.Entry(enrollment).State = EntityState.Modified;



            var team = new Teams
            {
                TeamId = enrollment.IrID,
                TeamName = intern.UserName,
                TeamPost = job.JobDesignation,
                TeamProfile = intern.UserProfile,
                CompanyId = enrollment.ccid,
                UniversityId = null
            };

            dbContext.Teams.Add(team);
            await dbContext.SaveChangesAsync(); 


            // Redirect to the Pending Enrollments page
            return RedirectToAction("ManageEnrollments");
        }

        [HttpPost]
        public async Task<IActionResult> RejectEnrollment(Guid enrollmentId)
        {
            // Retrieve the enrollment record
            var enrollment = await dbContext.InternRequest.FindAsync(enrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.Status = "Rejected";
            // Remove the enrollment record
            dbContext.Entry(enrollment).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            // Redirect to the Pending Enrollments page
            return RedirectToAction("ManageEnrollments");
        }




    }
}
