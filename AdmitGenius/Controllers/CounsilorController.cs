using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdmitGenius.Controllers
{
    public class CounsilorController : Controller
    {
        private readonly AdmitDBContext dbContext;

        public CounsilorController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public IActionResult CounsilorDashboard()
        {
            // Get the user ID from the session
            Guid cid = GetUserIdFromSession();


            var CounselorSubscribers = dbContext.CounselorSubscribers
                .Include(x => x.User)
                .Where(q => q.CounselorId == cid).ToList();

            var placed = CounselorSubscribers.Where(x => x.status == "placed").Count();
            var unplaced = CounselorSubscribers.Where(x => x.status == "paid").Count();

            ViewBag.placed = placed;
            ViewBag.unplaced = unplaced;

            if (CounselorSubscribers == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View(CounselorSubscribers);
        }

        public async Task<IActionResult> AddSubscription(Guid cid)
        {
            Guid userId = GetUserIdFromSession();
            Guid UniqueId = Guid.NewGuid();

            // Check if the subscription already exists
            var existingSubscription = dbContext.CounselorSubscribers
                .SingleOrDefault(cs => cs.UserId == userId && cs.CounselorId == cid);

            if (existingSubscription == null)
            {


                // Create a new subscription if it doesn't exist
                var newSubscription = new CounselorSubscribers
                {
                    CSID = UniqueId,
                    UserId = userId,
                    CounselorId = cid,
                    status = "paid",
                    LastMessage = " ",
                    GroupName = UniqueId.ToString(),
                };

                dbContext.CounselorSubscribers.Add(newSubscription);
                dbContext.SaveChanges();
                return RedirectToAction("SubscriptionAdded");
            }


            // Update the existing subscription if it exists
            existingSubscription.status = "paid";

            existingSubscription.CounselorId = cid;
            existingSubscription.GroupName = UniqueId.ToString();
            dbContext.CounselorSubscribers.Update(existingSubscription);
            dbContext.SaveChanges();

            return RedirectToAction("SubscriptionModified");

        }




        public IActionResult SubscriptionAdded()
        {
            return View();
        }

        public IActionResult SubscriptionModified()
        {
            return View();
        }




        public IActionResult CounselorChat(Guid cid)
        {
            Guid userId = GetUserIdFromSession();

            var subscriptionDetails = dbContext.CounselorSubscribers
                .Include(cs => cs.User)  // Assuming there's a navigation property named 'User' in CounselorSubscribers
                .Include(cs => cs.Counselors)  // Assuming there's a navigation property named 'User' in CounselorSubscribers
                .Where(cs => cs.UserId == userId && cs.CounselorId == cid && cs.status == "paid")
                .SingleOrDefault();

            return View(subscriptionDetails);
        }


        public IActionResult StudentChat()
        {
            Guid counselorId = GetUserIdFromSession();

            var counselorSubscribersList = dbContext.CounselorSubscribers
                .Include(cs => cs.User)
                .Where(cs => cs.CounselorId == counselorId)
                .ToList();

            return View(counselorSubscribersList);
        }



        /* public IActionResult CounselorChat1(Guid cid)
         {
             Guid Userid = GetUserIdFromSession();

             // Retrieve the list of approved counselors from the database
             var cm = dbContext.CounselorSubscribers
                 .Where(c => c.CounselorId == cid)
                 .SingleOrDefault();

             // Pass the list of approved counselors to the view
             return View(cm);
         }*/





        public async Task<IActionResult> CounsilorPage_old(Guid CounselorId)
        {
            // Retrieve the list of approved counselors from the database
            var approvedCounselors = await dbContext.Counselors
                .Include(c => c.Education)
                .Include(c => c.Experiences)
                .Where(c => c.RequestStatus.ToLower() == "approved")
                .FirstOrDefaultAsync(c => c.CounselorId == CounselorId);

            // Pass the list of approved counselors to the view
            return View(approvedCounselors);
        }

        public async Task<IActionResult> CounsilorPage(Guid CounselorId)
        {
            // Get the current user's Id from the session
            Guid currentUserId = GetUserIdFromSession();


            if (currentUserId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }

            // Retrieve the counselor subscriber from the database
            var counselorSubscriber = await dbContext.CounselorSubscribers
                .FirstOrDefaultAsync(cs => cs.CounselorId == CounselorId && cs.UserId == currentUserId);

            // Check if the counselor subscriber exists and is paid
            if (counselorSubscriber != null && counselorSubscriber.status.ToLower() == "paid")
            {
                // Set a flag to indicate that the counselor is paid
                ViewBag.IsCounselorPaid = true;
            }
            else
            {
                // Set a flag to indicate that the counselor is not paid
                ViewBag.IsCounselorPaid = false;
            }

            // Retrieve the counselor details
            var counselorDetails = await dbContext.Counselors
                .Include(c => c.Education)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.CounselorId == CounselorId);

            // Pass the counselor details to the view
            return View(counselorDetails);
        }

        public async Task<IActionResult> AllCounsilor()
        {
            // Retrieve the list of approved counselors from the database
            var approvedCounselors = await dbContext.Counselors
                .Where(c => c.RequestStatus.ToLower() == "approved")
                .ToListAsync();

            // Pass the list of approved counselors to the view
            return View(approvedCounselors);
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
                Github = user.Github,
                FaceBook = user.FaceBook,
                Twitter = user.Twitter,

                Skills = user.Skills,
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
            var couns = await dbContext.Counselors.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Update user details
            user.UserName = editViewModel.UserName;
            couns.CounselorFirstName = editViewModel.UserName;

            user.UserEmail = editViewModel.UserEmail;
            couns.CounselorEmail = editViewModel.UserEmail;
            user.UserPhone = editViewModel.UserPhone;
            couns.CounselorPhone = editViewModel.UserPhone;

            user.UserCountry = editViewModel.UserCountry;
            couns.CounselorCountry = editViewModel.UserCountry;

            user.UserFullLocation = editViewModel.UserFullLocation;
            couns.CounselorFullLocation = editViewModel.UserFullLocation;

            user.UserGraduation = editViewModel.UserGraduation;
            couns.CounselorDegree = editViewModel.UserGraduation;


            user.LinkedIn = editViewModel.LinkedIn;
            couns.CounselorInsta = editViewModel.Twitter;

            user.Github = editViewModel.Github;


            user.FaceBook = editViewModel.FaceBook;
            couns.CounselorFb = editViewModel.FaceBook;

            user.Twitter = editViewModel.Twitter;
            couns.CounselorX = editViewModel.Twitter;

            user.Skills = editViewModel.Skills;

            user.UserBio = editViewModel.UserBio;
            couns.CounselorBio = editViewModel.UserBio;


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
                        couns.CounselorProfilePicture = $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            dbContext.Entry(user).State = EntityState.Modified;
            dbContext.Entry(couns).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return RedirectToAction("CounsilorDashboard", "Counsilor");

        }


        private Guid GetUserIdFromSession()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }
}
