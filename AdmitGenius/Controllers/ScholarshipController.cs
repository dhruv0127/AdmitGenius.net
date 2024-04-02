using AdmitGenius.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.IdentityModel.Tokens;

namespace AdmitGenius.Controllers
{
    public class ScholarshipController : Controller
    {
        private readonly AdmitDBContext dbContext;

        public ScholarshipController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<IActionResult> AllScholarship(string search, DateTime dt)
        {
            // Retrieve the list of approved counselors from the database
            var approvedScholarship = await dbContext.Scholarship
                .Where(c => c.RequestStatus.ToLower() == "approved")
                .ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                approvedScholarship = approvedScholarship
                    .Where(c => 
                        c.ScholarshipName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.ScholarshipDescription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.ScholarshipPrice.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.ScholarshipLink.Contains(search, StringComparison.OrdinalIgnoreCase) 
                        ).ToList();
            }

            if (dt != default(DateTime))
            {
                DateTime startDate = dt.Date;
                DateTime endDate = startDate.AddDays(1); 

                approvedScholarship = approvedScholarship
                    .Where(c => c.ScholarshipPostedDate >= startDate && c.ScholarshipPostedDate < endDate)
                    .ToList();
            }


            // Pass the list of approved counselors to the view
            return View(approvedScholarship);
        }

        // Add new Student
        [HttpGet]
        public IActionResult AddSchoarship()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSchoarship(AddScholrshipViewModel scholarship)
        {

            var sclrsp = new Scholarship()
            {
                ScholrshipId = Guid.NewGuid(),
                ScholarshipName = scholarship.ScholarshipName,
                ScholarshipDescription = scholarship.ScholarshipDescription,
                ScholarshipPrice = scholarship.ScholarshipPrice,
                RequestStatus = "pending",
                ScholarshipPostedDate = DateTime.Now,
                ScholarshipLink = scholarship.ScholarshipLink
            };


            if (scholarship.ScholrshipImage != null)
            {
                sclrsp.ScholrshipImage = HandleFileUploads(scholarship.ScholrshipImage);
            }


            await dbContext.Scholarship.AddAsync(sclrsp);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("AllScholarship");
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

    }
}
