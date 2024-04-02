using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdmitGenius.Controllers
{
    public class CourseController : Controller
    {
        private readonly AdmitDBContext adminDbContext;

        public CourseController(AdmitDBContext adminDbContext)
        {
            this.adminDbContext = adminDbContext;
        }

        public async Task<IActionResult> CoursePage(Guid courseId)
        {
            // Retrieve the current course
            var currentCourse = await adminDbContext.Courses
                .Include(c => c.University)
                .Include(c => c.Hashtags)  // Include the Hashtags relationship
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (currentCourse == null)
            {
                return NotFound();
            }

            // Retrieve the related courses with the same hashtags
            var relatedCourses = adminDbContext.Courses
                .Include(c => c.University)
                .Include(c => c.Hashtags)  // Include the Hashtags relationship
                .AsEnumerable() // Switch to client-side evaluation
                .Where(c => c.CourseId != courseId && c.Hashtags.Any(h => currentCourse.Hashtags.Any(ch => ch.Name == h.Name)))
                .ToList();

            // Retrieve the university (you need to implement this logic)
            var university = await adminDbContext.Universities
                .FirstOrDefaultAsync(u => u.UniversityId == currentCourse.UniversityId);

            // Retrieve the counselors asynchronously
            var approvedCounselors = await adminDbContext.Counselors
                .Where(c => c.RequestStatus.ToLower() == "approved")
                .ToListAsync();

            var viewModel = new CourseViewModel
            {
                CourseDetails = currentCourse,
                UniversityDetails = university,
                Counselors = approvedCounselors,
                RelatedCourses = relatedCourses
            };

            return View(viewModel);
        }






        [HttpPost]
        public async Task<IActionResult> Enroll(EnrollmentViewModel model)
        {
            Guid studentId = GetuserIdFromSession();

            if (studentId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }

            // Retrieve the user and eligibility test
            var user = await adminDbContext.Users
                .Include(u => u.EligibilityTest)
                .FirstOrDefaultAsync(u => u.UserId == studentId);

            if (user == null)
            {
                return NotFound();
            }


            // Check if the student is already enrolled in the course
            var existingEnrollment = await adminDbContext.StudentEnrollRequest
                .FirstOrDefaultAsync(e => e.StudentID == studentId && e.CourseID == model.CourseId);

            if (existingEnrollment != null)
            {
                return RedirectToAction("AlreadyEnrolled");
            }

            // Get the course details
            var courseDetails = await adminDbContext.Courses
                .Include(c => c.University)
                .FirstOrDefaultAsync(c => c.CourseId == model.CourseId);

            if (courseDetails == null)
            {
                return NotFound();
            }

            // Check eligibility criteria before enrollment
            if (!IsStudentEligibleForCourse(user.EligibilityTest, courseDetails))
            {
                return RedirectToAction("IneligibleForEnrollment");
            }


            // Create a new enrollment record
            var enrollment = new StudentEnrollRequest
            {
                Universityiid = courseDetails.UniversityId,
                StudentID = studentId,
                CourseID = model.CourseId,
                DateOfRequest = DateTime.Now,
                Status = "Pending",
                CounselorEmailID = model.CounselorEmailId
            };

            // Add the enrollment to the database
            adminDbContext.StudentEnrollRequest.Add(enrollment);
            await adminDbContext.SaveChangesAsync();

            // Redirect to the enrollment success page
            return RedirectToAction("EnrollmentSuccess", new { courseName = courseDetails.CourseName, universityName = courseDetails.University?.UniversityName });
        }

            private bool IsStudentEligibleForCourse(EligibilityTest eligibilityTest, Courses courseDetails)
            {
                // Check if StudyDestinationCountries is null or empty
               /* if (eligibilityTest.StudyDestinationCountries == null || !eligibilityTest.StudyDestinationCountries.Any())
                {
                    return false;
                }

                // 1. Check study destination country
                if (!eligibilityTest.StudyDestinationCountries.Any(country => country.CountryName == courseDetails.University.UniversityCountry))
                {
                    return false;
                }   */         


            // 2. Check budget
            if (eligibilityTest.Budget < courseDetails.CourseFees)
                {
                    TempData["IneligibilityReason"] = "Your budget is less as " + @courseDetails.CourseFees + " is Course fees";
                    return false;
                }

                /* 3. Check interests (assuming interests is a list)
                var desiredInterests = new List<string> { "Interest1", "Interest2" };
                if (!eligibilityTest.Interests.Any(i => desiredInterests.Contains(i.Name)))
                {
                    return false;
                }*/

                // 4. Check highest education level
                if (eligibilityTest.HighestEducationLevel != courseDetails.CourseMinimumRequirements)
                {
                    TempData["IneligibilityReason"] = "Your Education level doenst meet the requiremnt of course education level " ;
                    return false;
                }

            // 5. other exam scores 

           
            if (courseDetails.IELTS != null)
            {

                if (eligibilityTest.IELTS < courseDetails.IELTS)
                {
                    TempData["IneligibilityReason"] = "your IELTS grades doesnt meet the course minimum required grades";
                    return false;
                }
            }

            if (courseDetails.PTE != null)
            {

                if (eligibilityTest.PTE < courseDetails.PTE)
                {
                    TempData["IneligibilityReason"] = "your PTE grades doesnt meet the course minimum required grades";
                    return false;
                }
            }

            if (courseDetails.TOFEL != null)
            {

                if (eligibilityTest.TOFEL < courseDetails.TOFEL)
                {
                    TempData["IneligibilityReason"] = "your TOFEL grades doesnt meet the course minimum required grades";
                    return false;
                }
            }

            if (eligibilityTest.ACT != null)
            {

                if (eligibilityTest.ACT < courseDetails.ACT)
                {
                    TempData["IneligibilityReason"] = "your ACT grades doesnt meet the course minimum required grades";
                    return false;
                }
            }

            if (courseDetails.SAT != null)
            {

                if (eligibilityTest.SAT < courseDetails.SAT)
                {
                    TempData["IneligibilityReason"] = "your SAT grades doesnt meet the course minimum required grades";
                    return false;
                }
            }

            if (eligibilityTest.GRE != null)
            {

                if (eligibilityTest.GRE < courseDetails.GRE)
                {
                    TempData["IneligibilityReason"] = "your GRE grades doesnt meet the course minimum required grades";
                    return false;
                }
            }


            //technical grades

            float studentgrades;
            float coursegrades;

            switch (eligibilityTest.GradingSystem.ToLower())
            {
                case "us":
                case "canada":

                    studentgrades = ConvertGradesFromString(eligibilityTest.GradesInUS);
                    coursegrades = ConvertGradesFromString(courseDetails.MinimumRequirementsUS);

                    if (studentgrades < coursegrades)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }
                    break;

                case "uk":

                    studentgrades = ConvertGradesFromString(eligibilityTest.GradesInUK);
                    coursegrades = ConvertGradesFromString(courseDetails.MinimumRequirementsUK);

                    if (studentgrades < coursegrades)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }
                    break;

                case "germany":

                    if (eligibilityTest.GradesInGermany > courseDetails.MinimumRequirementsGermany)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }

                    break;

                case "france":
                    if (eligibilityTest.GradesInFrance < courseDetails.MinimumRequirementsFrance)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }
                    break;

                case "japan":
                    if (eligibilityTest.GradesInJapan < courseDetails.MinimumRequirementsJapan)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }
                    break;

                case "other":
                    if (eligibilityTest.GradesInOther < courseDetails.MinimumRequirementsOther)
                    {
                        TempData["IneligibilityReason"] = "your grades doesnt meet the course minimum required grades";
                        return false;
                    }
                    break;

                default:
                    break;

            }


            // If all criteria are met, the student is eligible
            return true;
            }

       
       public float ConvertGradesFromString(string ebt)
        {
            float grades;

            switch (ebt.ToUpper())
            {
                case "A*":
                    grades = 5.5f;
                    break;
                case "A":
                    grades = 5.0f;
                    break;
                case "B":
                    grades = 4.0f;
                    break;
                case "C":
                    grades = 3.0f;
                    break;
                case "D":
                    grades = 2.0f;
                    break;
                case "E":
                    grades = 1.0f;
                    break;
                default:
                    grades = 0.0f;
                    break;
            };
        return grades;
        }


        public IActionResult EnrollmentSuccess(string courseName, string universityName)
        {

            // Pass the course and university names to the view
            ViewBag.CourseName = courseName;
            ViewBag.UniversityName = universityName;

            return View();
        }


        private Guid GetuserIdFromSession()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            // Handle the case where the University ID is not found or not a valid GUID
            // For example, you might redirect the user to a login page.
            return Guid.Empty;
        }

        public IActionResult IneligibleForEnrollment()
        {
            string ineligibilityReason = TempData["IneligibilityReason"] as string;
            ViewBag.IneligibilityReason = ineligibilityReason;
            return View();
        }

    }
}
