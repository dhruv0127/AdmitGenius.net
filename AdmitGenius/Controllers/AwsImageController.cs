using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Microsoft.Identity.Client;

namespace AdmitGenius.Controllers
{
    public class AwsImageController : Controller
    {
        private readonly ILogger<AwsImageController> logger;
        private readonly AdmitDBContext adminDbContext;

        public AwsImageController(AdmitDBContext adminDbContext, ILogger<AwsImageController> lg)
        {
            this.adminDbContext = adminDbContext;
            logger = lg;
        }

        public IActionResult AllImages()
        {
            List<S3FileDetails> files = new List<S3FileDetails>();

            files = adminDbContext.S3FileDetails.ToList();

            return View(files);
        }

        



        //=============Upload==================//

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var request = new TransferUtilityUploadRequest
                    {
                        InputStream = memoryStream,
                        Key = file.FileName,
                        BucketName = "yourbucketname",
                        ContentType = file.ContentType,
                    };

                    var transferUtility = new TransferUtility(amazonS3Client);
                    await transferUtility.UploadAsync(request);
                }
            }

            S3FileDetails filedetails = new S3FileDetails();
            filedetails.Id = Guid.NewGuid();
            filedetails.FileName = file.FileName;
            filedetails.S3Url=$"https://yourbucketname.s3.amazonaws.com/{file.FileName}";

            adminDbContext.S3FileDetails.Add(filedetails);
            adminDbContext.SaveChanges();

            ViewBag.Sucess = "File Uploaded";

            return RedirectToAction("AllImages");
        }
    }
        
}
