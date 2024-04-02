using Microsoft.AspNetCore.Mvc;
using AdmitGenius.Data;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AdmitGenius.Models.ViewModels;
using Amazon.S3.Transfer;
using Amazon.S3;

namespace AdmitGenius.Controllers
{
    public class StudentProfileController : Controller
    {
        private readonly AdmitDBContext dbContext;


        public StudentProfileController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            Guid userId = GetUserIdFromSession();

            var user = dbContext.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.University)
                .Include(u=> u.InternRequest)
                    .ThenInclude(i=>i.Job)
                    .ThenInclude(i=>i.Company)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserRole != "Student")
            {
                return RedirectToAction("Login", "Users");
            }

            return View(user);
        }


        public IActionResult Resume(Guid userId)
        {
           
            // Check if the user ID is not valid
            if (userId == Guid.Empty)
            {
                userId = GetUserIdFromSession();
            }

            var user = dbContext.Users
                .Include(u => u.Experiences)
                .Include(u => u.Education)
                .Include(u => u.Projects)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserRole != "Student")
            {
                return RedirectToAction("Login", "Users");
            }

            return View(user);
        }



        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            // Get the user ID from the session
            Guid userId = GetUserIdFromSession();

            // Check if the user ID is not valid
            if (userId == Guid.Empty)
            {
                return NotFound();
            }

            // Retrieve the user
            var user = await dbContext.Users.FindAsync(userId);

            // Check if the user exists
            if (user == null)
            {
                return NotFound();
            }

            // Map the user details to a view model
            var editUserViewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPassword = user.UserPassword,
                UserPhone = user.UserPhone,
                UserCountry = user.UserCountry,
                UserFullLocation = user.UserFullLocation,
                UserGraduation = user.UserGraduation,

                UserBio = user.UserBio,
                
                LinkedIn = user.LinkedIn,
                Github =   user.Github,
                FaceBook = user.FaceBook,
                Twitter =  user.Twitter,

                Skills =    user.Skills,
                resumePrefrence = user.resumePrefrence,
            };


            return View(editUserViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(EditUserViewModel editViewModel)
        {

            // Get the user ID from the session
            Guid userId = GetUserIdFromSession();

            // Retrieve the user
            if (userId == null)
            {
                return NotFound();
            }

            var user = await dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Update user details
            user.UserName = editViewModel.UserName;
            user.UserEmail = editViewModel.UserEmail;
            user.UserPhone = editViewModel.UserPhone;
            user.UserCountry = editViewModel.UserCountry;
            user.UserFullLocation = editViewModel.UserFullLocation;
            user.UserGraduation = editViewModel.UserGraduation;



            user.LinkedIn = editViewModel.LinkedIn;
            user.Github = editViewModel.Github;
            user.FaceBook = editViewModel.FaceBook;
            user.Twitter = editViewModel.Twitter;
            
            user.Skills = editViewModel.Skills;

            user.resumePrefrence = editViewModel.resumePrefrence;

            user.UserBio = editViewModel.UserBio;


            if (editViewModel.UserProfile != null)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UserProfile.CopyTo(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = $"{editViewModel.UserProfile.FileName}",
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UserProfile.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);

                        user.UserProfile = $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            if (editViewModel.resumeFileString != null)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.resumeFileString.CopyTo(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = $"{editViewModel.resumeFileString.FileName}",
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.resumeFileString.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);

                        user.resumeFileString = $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        public IActionResult AddProject()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProject(addProjectViewModel addProject)
        {

            Guid userId = GetUserIdFromSession();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }


            var project = new Projects()
            {
                ProjectId = Guid.NewGuid(),

                ProjectName = addProject.ProjectName,
                ProjectDescription = addProject.ProjectDescription,
                GithubRepoUrl = addProject.GithubRepoUrl,

                UserId = userId
            };


            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "StudentProfile");
        }



        [HttpGet]
        public IActionResult ExperienceDetails()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ExperienceDetails(addExperienceViewModel addExperiences)
        {

            Guid userId = GetUserIdFromSession();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }


            var userRole = HttpContext.Session.GetString("UserRole");


            var experiences = new Experiences()
            {
                ExpId = Guid.NewGuid(),
                ExpName = addExperiences.ExpName,
                ExpPostName = addExperiences.ExpPostName,
                ExpComapnyName = addExperiences.ExpComapnyName,
                ExpStartDate = addExperiences.ExpStartDate,
                ExpEndDate = addExperiences.ExpEndDate,


                Responsibilities = addExperiences.Responsibilities,


                UserId = userId,

            };


            if (userRole == "Student")
            {
                experiences.CounselorId = null;
                await dbContext.Experiences.AddAsync(experiences);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "StudentProfile");
            }
            else
            {
                experiences.CounselorId = userId;
                await dbContext.Experiences.AddAsync(experiences);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("AllCounsilor", "Counsilor");
            }



        }

        [HttpGet]
        public IActionResult EducationDetails()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EducationDetails(addEducationViewModel addEducation)
        {

            Guid userId = GetUserIdFromSession();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }

            var userRole = HttpContext.Session.GetString("UserRole");


            var education = new Education()
            {
                EducationId = Guid.NewGuid(),
                EducationName = addEducation.EducationName,
                SchoolName = addEducation.SchoolName,
                EducationStartDate = addEducation.EducationStartDate,
                EducationEndDate = addEducation.EducationEndDate,


                Percentage = addEducation.Percentage,
                SchoolCity = addEducation.SchoolCity,


                UserId = userId,

            };

            if (userRole == "Student")
            {
                education.CounselorId = null;
                await dbContext.Education.AddAsync(education);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "StudentProfile");
            }
            else
            {
                education.CounselorId = userId;
                await dbContext.Education.AddAsync(education);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("AllCounsilor", "Counsilor");
            }

        }


        public IActionResult Status()
        {
            Guid userId = GetUserIdFromSession();

            var user = dbContext.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.University)
                .Include(u => u.InternRequest)
                    .ThenInclude(i => i.Job)
                    .ThenInclude(i => i.Company)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserRole != "Student")
            {
                return RedirectToAction("Login", "Users");
            }

            return View(user);
        }






        private Guid GetUserIdFromSession()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            // Handle the case where the user ID is not found or not a valid GUID
            // For example, you might redirect the user to a login page.
            return Guid.Empty;
        }
    }
}
