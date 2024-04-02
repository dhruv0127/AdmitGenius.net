using AdmitGenius.Data;
using AdmitGenius.Models.ViewModels;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Amazon.S3.Transfer;
using Amazon.S3;

namespace AdmitGenius.Controllers
{
    public class UsersController : Controller
    {
        private readonly AdmitDBContext adminDbContext;
        IWebHostEnvironment hostingenvironment;
        private readonly DataAccessLayer _dataAccessLayer;

        public UsersController(AdmitDBContext adminDbContext, IWebHostEnvironment hc, DataAccessLayer dataAccessLayer)
        {
            this.adminDbContext = adminDbContext;
            hostingenvironment = hc;
            _dataAccessLayer = dataAccessLayer;
        }

        public IActionResult RequestSuccessful()
        {
            return View();
        }

        public IActionResult RedirectTo()
        {
            return View();
        }






        // Add new Student
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AddUserViewModel addUserRequest)
        {
            var user = new Users()
            {
                UserId = Guid.NewGuid(),
                UserName = addUserRequest.UserName,
                UserEmail = addUserRequest.UserEmail,
                UserPassword = addUserRequest.UserPassword,
                UserProfile = "https://yourbucketname.s3.amazonaws.com/dummyprofile.jpg",
                UserRole = "Student"
            };


            //add the userid in session               
            HttpContext.Session.SetString("UserId", user.UserId.ToString());

            if (user.UserName != null)
            {
                HttpContext.Session.SetString("UserName", user.UserName);
            }

            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetString("UserRole", user.UserRole);


            if (user.UserProfile != null)
            {
                HttpContext.Session.SetString("UserProfile", user.UserProfile);
            }


            await adminDbContext.Users.AddAsync(user);
            await adminDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "StudentProfile");
        }




        // Login action
        [HttpGet]
        public IActionResult Login()
        {
            var loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginModel)
        {
            if (loginModel.UserEmail == "agadmin@gmail.com" && loginModel.UserPassword == "@Agadmin99799")
            {
                return RedirectToAction("AgDashboard", "AgAdmin");
            }

            var user = adminDbContext.Users.SingleOrDefault(u => u.UserEmail == loginModel.UserEmail && u.UserPassword == loginModel.UserPassword);

            if (user != null)
            {
                //add the userid in session               
                HttpContext.Session.SetString("UserId", user.UserId.ToString());

                if (user.UserName != null)
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                }

                HttpContext.Session.SetString("UserEmail", user.UserEmail);
                HttpContext.Session.SetString("UserRole", user.UserRole);


                if (user.UserProfile != null)
                {
                    HttpContext.Session.SetString("UserProfile", user.UserProfile);
                }

                // Determine the user role directly from the database
                switch (user.UserRole)
                {
                    case "Student":
                        return RedirectToAction("Index", "StudentProfile");

                    case "University":
                        return RedirectToAction("UniversityDashboard", "University");

                    case "Counselor":
                        return RedirectToAction("CounsilorDashboard", "Counsilor");

                    case "Company":
                        return RedirectToAction("Dashboard", "Company");

                    case "CommunityAdmin":
                        return RedirectToAction("CommunityAdminDashboard", "Community");
                }
            }


            return View("Login");
        }




        //Company Registration Request 

        [HttpGet]
        public IActionResult CompanyRegisterRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompanyRegisterRequest(AddComapnyViewModel addComapnyViewModel)
        {
            var comapnyId = Guid.NewGuid();
            string mainImageFilename = "";
            string companylogo = "";

            // Handle the main university image
            if (addComapnyViewModel.CompanyImage != null)
            {
                mainImageFilename = HandleMainImageUpload(addComapnyViewModel.CompanyImage);
            }
            if (addComapnyViewModel.CompanyLogo != null)
            {
                companylogo = HandleMainImageUpload(addComapnyViewModel.CompanyLogo);
            }


            var company = new Company()
            {
                CompanyId = comapnyId,
                CompanyName = addComapnyViewModel.CompanyName,
                CompanyTagline = addComapnyViewModel.CompanyTagline,
                CompanyDescription = addComapnyViewModel.CompanyDescription,

                CompanyRegisterDate = addComapnyViewModel.CompanyRegisterDate,

                CompanyCountry = addComapnyViewModel.CompanyCountry,
                CompanyLocation = addComapnyViewModel.CompanyLocation,


                TotalClients = 0,
                TotalProjects = 0,
                TotalWorkers = 0,



                //featres
                Feature1 = addComapnyViewModel.Feature1,
                Feature2 = addComapnyViewModel.Feature2,
                Feature3 = addComapnyViewModel.Feature3,
                Feature4 = addComapnyViewModel.Feature4,
                Feature5 = addComapnyViewModel.Feature5,
                Feature6 = addComapnyViewModel.Feature6,


                //service and feature titile


                TitleOfFeature1 = addComapnyViewModel.TitleOfFeature1,
                TitleOfFeature2 = addComapnyViewModel.TitleOfFeature2,
                TitleOfFeature3 = addComapnyViewModel.TitleOfFeature3,
                TitleOfFeature4 = addComapnyViewModel.TitleOfFeature4,
                TitleOfFeature5 = addComapnyViewModel.TitleOfFeature5,
                TitleOfFeature6 = addComapnyViewModel.TitleOfFeature6,



                //logitude
                logitude = addComapnyViewModel.logitude,
                latitude = addComapnyViewModel.latitude,

                CompanyOfficalEmailAddress = addComapnyViewModel.CompanyOfficalEmailAddress,
                CompanyPassword = addComapnyViewModel.CompanyPassword,

                CompanyWebsite = addComapnyViewModel.CompanyWebsite,


                CompanyImage = mainImageFilename,
                CompanyLogo = companylogo,


                RequestStatus = "Pending",
            };

            await adminDbContext.Company.AddAsync(company);
            await adminDbContext.SaveChangesAsync();
            return RedirectToAction("RequestSuccessful");
        }





        //University Registration Request 

        [HttpGet]
        public IActionResult UniversityRegisterRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UniversityRegisterRequest(AddUniversityViewModel addUniversityRequest)
        {
            var universityId = Guid.NewGuid();
            string mainImageFilename = "";

            // Handle the main university image
            if (addUniversityRequest.UniversityImageFile != null)
            {
                mainImageFilename = HandleMainImageUpload(addUniversityRequest.UniversityImageFile);
            }

            // Handle additional university images
            List<string> additionalImagePaths = HandleImageUpload(addUniversityRequest.AdditionalImagesFiles);


            var universities = new Universities()
            {
                UniversityId = universityId,
                UniversityName = addUniversityRequest.UniversityName,
                UniversityTagline = addUniversityRequest.UniversityTagline,
                UniversityDescription = addUniversityRequest.UniversityDescription,
                UniversityImage = mainImageFilename,

                // Set additional image paths in the Universities model
                UniversityImage2 = additionalImagePaths.Count > 0 ? additionalImagePaths[0] : null,
                UniversityImage3 = additionalImagePaths.Count > 1 ? additionalImagePaths[1] : null,
                UniversityImage4 = additionalImagePaths.Count > 2 ? additionalImagePaths[2] : null,
                UniversityImage5 = additionalImagePaths.Count > 3 ? additionalImagePaths[3] : null,
                UniversityImage6 = additionalImagePaths.Count > 4 ? additionalImagePaths[4] : null,

                UniversityCountry = addUniversityRequest.UniversityCountry,
                UniversityLocation = addUniversityRequest.UniversityLocation,
                UniversityType = addUniversityRequest.UniversityType,
                UniversityRegisterDate = addUniversityRequest.UniversityRegisterDate,
                UniversityOfficalEmailAddress = addUniversityRequest.UniversityOfficalEmailAddress,
                UniversityPassword = addUniversityRequest.UniversityPassword,
                RequestStatus = "Pending",


                //featres
                Feature1 = addUniversityRequest.Feature1,
                Feature2 = addUniversityRequest.Feature2,
                Feature3 = addUniversityRequest.Feature3,
                Feature4 = addUniversityRequest.Feature4,
                Feature5 = addUniversityRequest.Feature5,

                //leter of acceptance
                LetterofAcceptanceJtoA = addUniversityRequest.LetterofAcceptanceJtoA,
                LetterofAcceptanceMtoA = addUniversityRequest.LetterofAcceptanceMtoA,
                LetterofAcceptanceStoD = addUniversityRequest.LetterofAcceptanceStoD,

                //logitude
                logitude = addUniversityRequest.logitude,
                latitude = addUniversityRequest.latitude


            };

            await adminDbContext.Universities.AddAsync(universities);
            await adminDbContext.SaveChangesAsync();
            return RedirectToAction("RequestSuccessful");
        }




        // Helper method to handle image upload for additional images
        private List<string> HandleImageUpload(List<IFormFile> imageFiles)
        {
            List<string> s3Urls = new List<string>();

            if (imageFiles != null && imageFiles.Count > 0)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    foreach (var imageFile in imageFiles)
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

                            s3Urls.Add($"https://yourbucketname.s3.amazonaws.com/{request.Key}");
                        }
                    }
                }
            }

            return s3Urls.Count > 0 ? s3Urls : null;
        }

        private string HandleMainImageUpload(IFormFile imageFile)
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






        //Counselor Registration Request 

        [HttpGet]
        public IActionResult CounselorRegisterRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CounselorRegisterRequest(AddCounselorViewModel addCounselorRequest)
        {
            var counselorId = Guid.NewGuid();

            var counselor = new Counselors()
            {
                CounselorId = counselorId,
                CounselorFirstName = addCounselorRequest.CounselorFirstName,
                CounselorLastName = addCounselorRequest.CounselorLastName,
                CounselorEmail = addCounselorRequest.CounselorEmail,
                CounselorPassword = addCounselorRequest.CounselorPassword,

                CounselorGender = addCounselorRequest.CounselorGender,
                CounselorBirthday = addCounselorRequest.CounselorBirthday,

                CounselorBio = addCounselorRequest.CounselorBio,

                TotalPlacedStudent = 0,

                CounselorPhone = addCounselorRequest.CounselorPhone,
                CounselorCountry = addCounselorRequest.CounselorCountry,
                CounselorFullLocation = addCounselorRequest.CounselorFullLocation,

                CounselorDegree = addCounselorRequest.CounselorDegree,
                CounselorUniversity = addCounselorRequest.CounselorUniversity,
                CounselorGraduationYear = addCounselorRequest.CounselorGraduationYear,
                WorkingHours = addCounselorRequest.WorkingHours,
                DaysAvailable = addCounselorRequest.DaysAvailable,
                MainLanguage = addCounselorRequest.MainLanguage,
                SecondaryLanguage = addCounselorRequest.SecondaryLanguage,
                CounselorFees = addCounselorRequest.CounselorFees,

                CounselorInsta = addCounselorRequest.CounselorInsta,
                CounselorFb = addCounselorRequest.CounselorFb,
                CounselorX = addCounselorRequest.CounselorX,


                CurrentComapny = addCounselorRequest.CurrentComapny,


                RequestStatus = "Pending"
            };

            // Handle the counselor profile photo
            if (addCounselorRequest.CounselorProfilePhoto != null)
            {
                counselor.CounselorProfilePicture = HandleImageUpload(addCounselorRequest.CounselorProfilePhoto);
            }

            await adminDbContext.Counselors.AddAsync(counselor);
            await adminDbContext.SaveChangesAsync();

            return RedirectToAction("RequestSuccessful");
        }

        // Helper method to handle image upload for counselor profile photo
        private string HandleImageUpload(IFormFile imageFile)
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


        [HttpPost]
        public IActionResult Logout()
        {
            // Clear user-related session variables
            HttpContext.Session.Clear();

            // You may want to redirect to the login page or any other page after signing out
            return RedirectToAction("Login");
        }





        [HttpGet]
        public IActionResult GetUserData(Guid userId)
        {
            var userData = _dataAccessLayer.GetUserById(userId);

            if (userData != null)
            {
                // Return user data as JSON
                return Json(new { UserName = userData.UserName, UserEmail = userData.UserEmail });
            }
            else
            {
                // Return an error or handle as needed
                return Json(new { ErrorMessage = "User not found" });
            }

        }

    }
}