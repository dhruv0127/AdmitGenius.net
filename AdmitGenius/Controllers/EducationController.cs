using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AdmitGenius.Controllers
{
    public class EducationController : Controller
    {

        private readonly AdmitDBContext dbContext;

        public EducationController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
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
