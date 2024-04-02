using AdmitGenius.Data;
using AdmitGenius.Hubs;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using Amazon.Util;

namespace AdmitGenius.Controllers
{
    public class CommunityController : Controller
    {
        private readonly AdmitDBContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;

        public CommunityController(AdmitDBContext dbContext, IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }


        [HttpPost]
        public async Task<string> UploadImageToS3(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var amazonS3Client = new AmazonS3Client("YOUR ID", "YOUR KEY", Amazon.RegionEndpoint.USEast1))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);

                        var request = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = $"{Guid.NewGuid()}-{file.FileName}", // Ensure unique key for each file
                            BucketName = "yourbucketname",
                            ContentType = file.ContentType,
                        };

                        var transferUtility = new TransferUtility(amazonS3Client);
                        await transferUtility.UploadAsync(request);

                        // Return the S3 URL of the uploaded image
                        return $"https://yourbucketname.s3.amazonaws.com/{request.Key}";
                    }
                }
            }

            return null;
        }


        public IActionResult AllCommunities()
        {
            var community = dbContext.Community.ToList();

            return View(community);
        }

        [HttpGet]
        public IActionResult CreateCommunity()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(AddCommunity addcommunity)
        {
            var user = GetUserFromSession();

            if (user == null)
            {
                RedirectToAction("Login", "Users");
            }

            Guid guidID = Guid.NewGuid();

            var Community = new Community()
            {
                CommunityId = guidID,
                CommunityName = addcommunity.CommunityName,
                CommunityDescription = addcommunity.CommunityDescription,
                CommunityCreationDate = DateTime.Now,
                GroupAdminId = user.UserId,
                GroupAdminName = user.UserName,

            };
            var cm = new CommunityMember()
            {
                CMID = Guid.NewGuid(),
                UserId = user.UserId,
                CommunityId = guidID,
                status = "admin"

            };

            if (addcommunity.CommunityImage != null)
            {
                Community.CommunityImage = HandleMainImageUpload(addcommunity.CommunityImage);
            }


            await dbContext.CommunityMember.AddAsync(cm);
            await dbContext.Community.AddAsync(Community);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("AllCommunities");
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


        public IActionResult CommunityAdminDashboard()
        {
            var user = GetUserFromSession();

            if (user == null)
            {
                RedirectToAction("Login", "Users");
            }

            var groups = dbContext.Community
                .Where(c => c.CommunityMember.Any(cm => cm.UserId == user.UserId && cm.status == "admin"))
                .ToList();


            return View(groups);
        }
        public IActionResult ManageCommunity(Guid cid)
        {
            var community = dbContext.Community
                .Include(c => c.CommunityMember)
                .ThenInclude(cm => cm.User)
                .FirstOrDefault(c => c.CommunityId == cid);

            if (community == null)
            {
                return NotFound(); // Or handle the case where the community is not found
            }

            return View(community);
        }



        public IActionResult RequestAdded(Guid cid)
        {
            var comm = dbContext.Community.Where(c => c.CommunityId == cid).FirstOrDefault();
            return View(comm);
        }
        public IActionResult RequestExists()
        {
            return View();
        }


        public async Task<IActionResult> AddRequestForJoin(Guid cid)
        {
            var user = GetUserFromSession();

            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Check if the user already has a pending request to join this community
            var existingRequest = await dbContext.CommunityMember
                .FirstOrDefaultAsync(cm => cm.UserId == user.UserId && cm.CommunityId == cid);

            if (existingRequest != null)
            {
                if (existingRequest.status == "approved")
                {
                    // You may want to return some message to inform the user that their request is already pending.
                    return RedirectToAction("index2");
                }

                if (existingRequest.status == "pending")
                {
                    // You may want to return some message to inform the user that their request is already pending.
                    return RedirectToAction("RequestExists");
                }
            }

            var cm = new CommunityMember
            {
                CMID = Guid.NewGuid(),
                UserId = user.UserId,
                CommunityId = cid,
                status = "pending"
            };

            await dbContext.CommunityMember.AddAsync(cm);
            await dbContext.SaveChangesAsync();

            // Pass the cid as a route value
            return RedirectToAction("RequestAdded", new { cid = cid });
        }


        //public async Task<IActionResult> AddRequestForJoin(Guid cid)
        //{
        //    var user = GetUserFromSession();

        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Users");
        //    }

        //    // Check if the user already has a pending request to join this community
        //    var existingRequest = await dbContext.CommunityMember
        //        .FirstOrDefaultAsync(cm => cm.UserId == user.UserId && cm.CommunityId == cid);

        //    if (existingRequest != null)
        //    {
        //        ViewBag.RequestExists = "Exists";
        //    }
        //    else
        //    {
        //        ViewBag.RequestExists = "NotExists";

        //        var cm = new CommunityMember
        //        {
        //            CMID = Guid.NewGuid(),
        //            UserId = user.UserId,
        //            CommunityId = cid,
        //            status = "pending"
        //        };

        //        await dbContext.CommunityMember.AddAsync(cm);
        //        await dbContext.SaveChangesAsync();
        //    }

        //    return View();
        //}




        [HttpPost]
        public IActionResult ApproveMember(Guid cid, Guid memberId)
        {
            var communityMember = dbContext.CommunityMember
                .FirstOrDefault(cm => cm.CommunityId == cid && cm.UserId == memberId && cm.status == "pending");

            if (communityMember != null)
            {
                communityMember.status = "approved";
                dbContext.SaveChanges();
            }

            // Redirect back to the ManageCommunity page
            return RedirectToAction("ManageCommunity", new { cid });
        }

        [HttpPost]
        public IActionResult RejectMember(Guid cid, Guid memberId)
        {
            var communityMember = dbContext.CommunityMember
                .FirstOrDefault(cm => cm.CommunityId == cid && cm.UserId == memberId && cm.status == "pending");

            if (communityMember != null)
            {
                dbContext.CommunityMember.Remove(communityMember);
                dbContext.SaveChanges();
            }

            // Redirect back to the ManageCommunity page
            return RedirectToAction("ManageCommunity", new { cid });
        }

        [HttpPost]
        public IActionResult RemoveMember(Guid cid, Guid memberId)
        {
            var communityMember = dbContext.CommunityMember
                .FirstOrDefault(cm => cm.CommunityId == cid && cm.UserId == memberId && cm.status != "pending");

            if (communityMember != null)
            {
                dbContext.CommunityMember.Remove(communityMember);
                dbContext.SaveChanges();
            }

            // Redirect back to the ManageCommunity page
            return RedirectToAction("ManageCommunity", new { cid });
        }

        public IActionResult Index2()
        {
            var user = GetUserFromSession();

            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var groups = dbContext.Community
                .Where(c => c.CommunityMember.Any(cm => cm.UserId == user.UserId && cm.status == "approved"))
                .ToList();

            return View(groups);
        }
        [HttpPost]
        public async Task<IActionResult> JoinGroup(string groupName)
        {
            // Access the current user's connection id using HttpContext.ConnectionId
            var connectionId = HttpContext.Connection.Id;

            //await hubContext.Clients.All.SendAsync("ReceiveMessage", $"{connectionId} has joined the group {groupName}");
            return Ok();
        }

        public async Task<IActionResult> LeaveGroup(string groupName)
        {
            // Access the current user's connection id using HttpContext.ConnectionId
            var connectionId = HttpContext.Connection.Id;

            //await hubContext.Clients.All.SendAsync("ReceiveMessage", $"{connectionId} has left the group {groupName}");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToGroup(string groupName, string message)
        {
            var user = GetUserFromSession();

            if (user == null)
            {
                return RedirectToAction("Index", "StudentProfile");
            }

            // Access the current user's connection id using HttpContext.ConnectionId
            var connectionId = HttpContext.Connection.Id;

            await hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
            return Ok();
        }

        private Users GetUserFromSession()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var user = dbContext.Users.Find(userId);
                return user;
            }

            return null;
        }




        [HttpGet]
        public IActionResult GetLastMessageForGroup(Guid groupId)
        {
            // Retrieve the last message for the group from the database
            var lastMessage = dbContext.ChatMessages
                .Where(m => m.groupName == groupId.ToString())
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault();

            return Json(lastMessage);
        }


    }
}



