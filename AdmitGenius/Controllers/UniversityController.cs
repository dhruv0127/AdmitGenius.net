using AdmitGenius.Data;
using AdmitGenius.Models.ViewModels;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure;
using Microsoft.CodeAnalysis;
using Amazon.S3.Transfer;
using Amazon.S3;
using Azure.Core;

namespace AdmitGenius.Controllers
{
    public class UniversityController : Controller
    {
        private readonly AdmitDBContext adminDbContext;
        private readonly IWebHostEnvironment hostingEnvironment;

        public UniversityController(AdmitDBContext adminDbContext, IWebHostEnvironment hostingEnvironment)
        {
            this.adminDbContext = adminDbContext;
            this.hostingEnvironment = hostingEnvironment;
        }


               
       

        public IActionResult UniversityDashboard()
        {            
            Guid universityId = GetUniversityIdFromSession();

            if (universityId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View(universityId);
        }



        private Guid GetUniversityIdFromSession()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid universityId))
            {
                return universityId;
            }

            return Guid.Empty;
        }




        [HttpGet]
        public IActionResult AddNewCourses()
        {
            var model = new AddCoursesViewModel
            {
                EducationLevels = GetEducationLevels(),
                MinimumRequirements = GetMinimumRequirements(),
                Languages = GetLanguages()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCourses(AddCoursesViewModel addCourseRequest)
        {
            var educationLevels = GetEducationLevels();
            var minimumRequirements = GetMinimumRequirements();
            var languages = GetLanguages();

            Guid sessionUniversityId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var course = new Courses()
            {
                CourseId = Guid.NewGuid(),
                CourseName = addCourseRequest.CourseName,
                CourseDescription = addCourseRequest.CourseDescription,
                CourseDuration = addCourseRequest.CourseDuration,
                UniversityId = sessionUniversityId,
                CourseFees = addCourseRequest.CourseFees,
                ApplicationFee = addCourseRequest.ApplicationFee,
                CostOFLiving = addCourseRequest.CostOFLiving, 
                WhatWillYouLearn = addCourseRequest.WhatWillYouLearn,
                CourseDepartment = addCourseRequest.CourseDepartment,
                CourseSemesters = addCourseRequest.CourseSemesters,
                CourseEducationLevel = addCourseRequest.CourseEducationLevel,
                CourseMinimumRequirements = addCourseRequest.CourseMinimumRequirements,
                MinimumRequirementsUS = addCourseRequest.MinimumRequirementsUS, 
                MinimumRequirementsUK = addCourseRequest.MinimumRequirementsUK, 
                MinimumRequirementsGermany = addCourseRequest.MinimumRequirementsGermany, 
                MinimumRequirementsFrance = addCourseRequest.MinimumRequirementsFrance, 
                MinimumRequirementsJapan = addCourseRequest.MinimumRequirementsJapan, 
                MinimumRequirementsOther = addCourseRequest.MinimumRequirementsOther, 
                CourseLanguage = addCourseRequest.CourseLanguage,
                IELTS = addCourseRequest.IELTS,
                TOFEL = addCourseRequest.TOFEL,
                PTE = addCourseRequest.PTE,  
                ACT = addCourseRequest.ACT,  
                SAT = addCourseRequest.SAT,  
                GRE = addCourseRequest.GRE,  
                CourseEnrollDeadlineIntake1 = addCourseRequest.CourseEnrollDeadline,
                CourseStartingDateIntake1 = addCourseRequest.CourseStartingDateIntake1,
                CourseEnrollDeadlineIntake2 = addCourseRequest.CourseEnrollDeadlineIntake2,
                CourseStartingDateIntake2 = addCourseRequest.CourseStartingDateIntake2, 
                CourseEnrollDeadlineIntake3 = addCourseRequest.CourseEnrollDeadlineIntake3,
                CourseStartingDateIntake3 = addCourseRequest.CourseStartingDateIntake3
            };

            if (addCourseRequest.CourseImage != null)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        addCourseRequest.CourseImage.CopyTo(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = $"{addCourseRequest.CourseImage.FileName}",
                            BucketName = "yourbucketname",
                            ContentType = addCourseRequest.CourseImage.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);

                        course.CourseImage = $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }                
            }


            // Add hashtags to the course
            if (!string.IsNullOrEmpty(addCourseRequest.Hashtags))
            {
                var hashtagNames = addCourseRequest.Hashtags.Split(',').Select(tag => tag.Trim());

                foreach (var hashtagName in hashtagNames)
                {
                    // Check if the hashtag already exists, if not, create a new one
                    var existingHashtag = await adminDbContext.Hashtags.FirstOrDefaultAsync(h => h.Name == hashtagName);
                    if (existingHashtag == null)
                    {
                        existingHashtag = new Hashtag { Name = hashtagName };
                        adminDbContext.Hashtags.Add(existingHashtag);
                    }

                    // Associate the hashtag with the course
                    course.Hashtags.Add(existingHashtag);
                }
            }

            await adminDbContext.Courses.AddAsync(course);
            await adminDbContext.SaveChangesAsync();

            return RedirectToAction("UniversityDashboard");
        }
        private List<string> GetEducationLevels()
        {
            // You can fetch education levels from your database or define them manually
            return new List<string> { "Grade 1", "Grade 2", "Grade 3", "Grade 4", "Grade 5", "Grade 6", "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "Bachelor", "Master", "PhD" };
;
        }

        private List<string> GetMinimumRequirements()
        {
            // You can fetch minimum requirements from your database or define them manually
            return new List<string> {"Grade 1", "Grade 2", "Grade 3", "Grade 4", "Grade 5", "Grade 6", "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "Bachelor", "Master", "PhD" };
        }

        private List<string> GetLanguages()
        {
            // You can fetch languages from your database or define them manually
            return new List<string> {"English", "Mandarin Chinese", "Spanish", "Hindi", "Arabic", "Bengali", "Portuguese", "Russian", "Urdu", "Indonesian", "German", "Japanese", "Swahili", "Telugu", "French", "Marathi", "Vietnamese", "Korean", "Tamil", "Italian"};
        }






        [HttpGet]
        public async Task<IActionResult> EditUniversityDetails()
        {
            // Get the University ID from the session
            Guid universityId = GetUniversityIdFromSession();

            // Retrieve the university details
            var university = await adminDbContext.Universities.FindAsync(universityId);

            if (university == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Map the university details to a view model
            var editUniversityViewModel = new EditUniversityDetailsViewModel
            {
                UniversityId = university.UniversityId,
                UniversityName = university.UniversityName,
                UniversityTagline = university.UniversityTagline,
                UniversityDescription = university.UniversityDescription,
                UniversityCountry = university.UniversityCountry,
                UniversityLocation = university.UniversityLocation,
                UniversityRegisterDate = university.UniversityRegisterDate,
                UniversityImageLocation = university.UniversityImage,

                // Add the new properties
                //UniversityImage2 = university.UniversityImage2,
                //UniversityImage3 = university.UniversityImage3,
                //UniversityImage4 = university.UniversityImage4,
                //UniversityImage5 = university.UniversityImage5,
                //UniversityImage6 = university.UniversityImage6,

                Feature1 = university.Feature1,
                Feature2 = university.Feature2,
                Feature3 = university.Feature3,
                Feature4 = university.Feature4,
                Feature5 = university.Feature5,

                LetterofAcceptanceJtoA = university.LetterofAcceptanceJtoA,
                LetterofAcceptanceMtoA = university.LetterofAcceptanceMtoA,
                LetterofAcceptanceStoD = university.LetterofAcceptanceStoD,

                logitude = university.logitude,
                latitude = university.latitude
            };

            return View(editUniversityViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditUniversityDetails(EditUniversityDetailsViewModel editViewModel)
        {
            // Retrieve the university entity
            var university = await adminDbContext.Universities.FindAsync(editViewModel.UniversityId);

            if (university == null)
            {
                return RedirectToAction("Login", "Users");
            }



            //============Main image ================//

            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImageFile != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImageFile.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImageFile.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImageFile.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImageFile.FileName;
                university.UniversityImage = $"https://yourbucketname.s3.amazonaws.com/{FileName}";


            }



            //2
            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImage2 != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImage2.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImage2.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImage2.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImage2.FileName;
                university.UniversityImage2= $"https://yourbucketname.s3.amazonaws.com/{FileName}";

                
            }


            //3
            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImage3 != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImage3.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImage3.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImage3.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImage3.FileName;
                university.UniversityImage3 = $"https://yourbucketname.s3.amazonaws.com/{FileName}";


            }

            //4
            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImage4 != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImage4.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImage4.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImage4.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImage4.FileName;
                university.UniversityImage4 = $"https://yourbucketname.s3.amazonaws.com/{FileName}";


            }

            //5
            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImage5 != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImage5.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImage5.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImage5.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImage5.FileName;
                university.UniversityImage5 = $"https://yourbucketname.s3.amazonaws.com/{FileName}";


            }


            //6
            // Handle file upload if a new image is provided
            if (editViewModel.UniversityImage6 != null)
            {

                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        editViewModel.UniversityImage6.CopyTo(memoryStream);
                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = editViewModel.UniversityImage6.FileName,
                            BucketName = "yourbucketname",
                            ContentType = editViewModel.UniversityImage6.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);
                    }
                }

                String FileName = editViewModel.UniversityImage6.FileName;
                university.UniversityImage6 = $"https://yourbucketname.s3.amazonaws.com/{FileName}";


            }


            // Handle additional image uploads
            // university.UniversityImage2 = HandleMainImageUpload(editViewModel.UniversityImage2);
            // university.UniversityImage3 = HandleMainImageUpload(editViewModel.UniversityImage3);
            // university.UniversityImage4 = HandleMainImageUpload(editViewModel.UniversityImage4);
            // university.UniversityImage5 = HandleMainImageUpload(editViewModel.UniversityImage5);
            // university.UniversityImage6 = HandleMainImageUpload(editViewModel.UniversityImage6);


            // Update other details
            university.UniversityName = editViewModel.UniversityName;
            university.UniversityTagline = editViewModel.UniversityTagline;
            university.UniversityDescription = editViewModel.UniversityDescription;
            university.UniversityCountry = editViewModel.UniversityCountry;
            university.UniversityLocation = editViewModel.UniversityLocation;
            university.UniversityRegisterDate = editViewModel.UniversityRegisterDate;

            // Update features
            university.Feature1 = editViewModel.Feature1;
            university.Feature2 = editViewModel.Feature2;
            university.Feature3 = editViewModel.Feature3;
            university.Feature4 = editViewModel.Feature4;
            university.Feature5 = editViewModel.Feature5;

            // Update letter of acceptance
            university.LetterofAcceptanceJtoA = editViewModel.LetterofAcceptanceJtoA;
            university.LetterofAcceptanceMtoA = editViewModel.LetterofAcceptanceMtoA;
            university.LetterofAcceptanceStoD = editViewModel.LetterofAcceptanceStoD;

            // Update location details
            university.logitude = editViewModel.logitude;
            university.latitude = editViewModel.latitude;

            adminDbContext.Entry(university).State = EntityState.Modified;
            await adminDbContext.SaveChangesAsync();

            // Redirect to the university dashboard or another appropriate page
            return RedirectToAction("UniversityDashboard");
        }

        // Helper method to handle image upload for additional images
        private string HandleMainImageUpload(IFormFile imageFile)
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
                            Key = $"{Guid.NewGuid()}_{imageFile.FileName}",
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






        public async Task<IActionResult> UniversityPage(Guid id)
        {
            var universityWithCourses = await adminDbContext.Universities
                .Include(u => u.Courses)
                .Include(u => u.Teams)
                .FirstOrDefaultAsync(u => u.UniversityId == id);

            if (universityWithCourses == null)
            {
                return NotFound();
            }

            return View(universityWithCourses);
        }






        /* Studnet Review  */

        public async Task<IActionResult> ReviewStudents()
        {
            // Get the University ID from the session
            Guid universityId = GetUniversityIdFromSession();

            // Retrieve all pending enrollments for the current university
            var pendingEnrollments = await adminDbContext.StudentEnrollRequest
                .Where(e => e.Universityiid == universityId && e.Status == "Pending")
                .Include(e => e.Course)  // Use the Course navigation property
                .Include(e => e.Student) // Use the Student navigation property
                .ToListAsync();



            // Pass the pending enrollments to the view
            return View(pendingEnrollments);
        }


        [HttpPost]
        public async Task<IActionResult> AcceptEnrollment(Guid enrollmentId)
        {
            // Retrieve the enrollment record
            var enrollment = await adminDbContext.StudentEnrollRequest.FindAsync(enrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            // Update the status to "Approved"
            enrollment.Status = "Approved";
            adminDbContext.Entry(enrollment).State = EntityState.Modified;
            await adminDbContext.SaveChangesAsync();

            // Redirect to the Pending Enrollments page
            return RedirectToAction("ReviewStudents");
        }

        [HttpPost]
        public async Task<IActionResult> RejectEnrollment(Guid enrollmentId)
        {
            // Retrieve the enrollment record
            var enrollment = await adminDbContext.StudentEnrollRequest.FindAsync(enrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            // Remove the enrollment record
            adminDbContext.StudentEnrollRequest.Remove(enrollment);
            await adminDbContext.SaveChangesAsync();

            // Redirect to the Pending Enrollments page
            return RedirectToAction("ReviewStudents");
        }





        /* Approved Student */       

        public async Task<IActionResult> ApprovedStudents()
        {
            // Get the University ID from the session
            Guid universityId = GetUniversityIdFromSession();

            // Retrieve all approved enrollments for the current university
            var approvedEnrollments = await adminDbContext.StudentEnrollRequest
                .Where(e => e.Universityiid == universityId && e.Status == "Approved")
                .Include(e => e.Course)
                .Include(e => e.Student)
                .ToListAsync();

            // Pass the approved enrollments to the view
            return View(approvedEnrollments);
        }



        /* Add Teachers */

        [HttpGet]
        public IActionResult AddNewTeacher()
        {            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewTeacher(AddTeamViewModel addTeamViewModel)
        {

            Guid sessionUniversityId = GetUniversityIdFromSession();

            var teams = new Teams()
            {
               TeamId = Guid.NewGuid(),
               TeamName = addTeamViewModel.TeamName,    
               TeamPost = addTeamViewModel.TeamPost,

               UniversityId = sessionUniversityId,
               CompanyId = null
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

            await adminDbContext.Teams.AddAsync(teams);
            await adminDbContext.SaveChangesAsync();

            return RedirectToAction("UniversityDashboard");
        }
    }
}