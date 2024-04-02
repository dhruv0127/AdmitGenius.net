using AdmitGenius.Data;
using AdmitGenius.Models.ViewModels;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdmitGenius.Controllers
{
    public class ExperienceController : Controller
    {

        private readonly AdmitDBContext dbContext;

        public ExperienceController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
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
