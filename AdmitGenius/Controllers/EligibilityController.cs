using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdmitGenius.Controllers
{
    public class EligibilityController : Controller
    {
        private readonly AdmitDBContext adminDbContext;

        public EligibilityController(AdmitDBContext adminDbContext)
        {
            this.adminDbContext = adminDbContext;
        }


        public IActionResult AptitudeTest()
        {
            return View();
        }
          public IActionResult AptitudeTestResult()
        {
            return View();
        }



            
        public IActionResult EligibilityTest()
        {
            var model = new EligibilityTestViewModel
            {
                AvailableCountries = GetCountries(),
                AvailableInterests = GetInterests(),
                AvailableEducationLevels = GetEducationLevels()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EligibilityTest(EligibilityTestViewModel etvmodel)
        {
            var userId = GetUserIdFromSession();
            var existingUser = await adminDbContext.Users
                .Include(u => u.EligibilityTest)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingUser != null)
            {
                var newEligibilityTest = new EligibilityTest
                {
                    StudyDestinationCountries = ConvertToStudyDestinationCountryList(etvmodel.StudyDestinationCountries),
                    Budget = etvmodel.Budget,
                    Interests = ConvertToInterestList(etvmodel.Interests),
                    HighestEducationLevel = etvmodel.HighestEducationLevel,
                    GradingSystem = etvmodel.GradingSystem,
                    IELTS = etvmodel.IELTS,
                    TOFEL = etvmodel.TOFEL,
                    PTE = etvmodel.PTE,
                    ACT = etvmodel.ACT,
                    SAT = etvmodel.SAT,
                    GRE = etvmodel.GRE,
                };


                switch (etvmodel.GradingSystem)
                {
                    case "us":
                    case "canada":
                    case "uk":
                        newEligibilityTest.GradesInUS = etvmodel.LastEducationGrade;
                        break;

                    case "germany":
                    case "france":
                    case "japan":
                    case "other":
                        if (float.TryParse(etvmodel.LastEducationGrade, out float grade))
                        {
                            switch (etvmodel.GradingSystem)
                            {
                                case "germany":
                                    newEligibilityTest.GradesInGermany = grade;
                                    break;

                                case "france":
                                    newEligibilityTest.GradesInFrance = grade;
                                    break;

                                case "japan":
                                    newEligibilityTest.GradesInJapan = grade;
                                    break;

                                case "other":
                                    newEligibilityTest.GradesInOther = grade;
                                    break;
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Invalid grade format for {etvmodel.GradingSystem}. Please enter a valid numeric grade.";
                            return RedirectToAction("GradeError", "Error");
                        }
                        break;

                    default:
                        TempData["ErrorMessage"] = "Invalid grading system. Please select a valid grading system.";
                        return RedirectToAction("GradeError", "Error");
                }

                existingUser.EligibilityTest = newEligibilityTest;
                existingUser.UserGraduation = etvmodel.HighestEducationLevel;

                await adminDbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return NotFound();
        }



        private List<StudyDestinationCountry> ConvertToStudyDestinationCountryList(List<string> countryNames)
        {
            if (countryNames == null)
            {
                return null;
            }

            return countryNames
                .Where(countryName => !string.IsNullOrWhiteSpace(countryName)) // Filter out null or empty values
                .Select(countryName => new StudyDestinationCountry { CountryName = countryName })
                .ToList();
        }


        private List<Interest> ConvertToInterestList(List<string> interestNames)
        {
            return interestNames?.Select(interestName => new Interest { Name = interestName }).ToList();
        }


        private List<SelectListItem> GetCountries()
        {
            // Replace this with your logic to fetch countries, e.g., from a database
            // For demonstration purposes, a simple hard-coded list is used here.
            var countries = new List<SelectListItem>
            {
                new SelectListItem { Value = "United States", Text = "United States" },
new SelectListItem { Value = "United Kingdom", Text = "United Kingdom" },
new SelectListItem { Value = "Canada", Text = "Canada" },
new SelectListItem { Value = "Australia", Text = "Australia" },
new SelectListItem { Value = "Germany", Text = "Germany" },
new SelectListItem { Value = "France", Text = "France" },
new SelectListItem { Value = "Japan", Text = "Japan" },
new SelectListItem { Value = "China", Text = "China" },
new SelectListItem { Value = "Brazil", Text = "Brazil" },
new SelectListItem { Value = "India", Text = "India" },
new SelectListItem { Value = "Russia", Text = "Russia" },
new SelectListItem { Value = "South Africa", Text = "South Africa" },
new SelectListItem { Value = "Mexico", Text = "Mexico" },
new SelectListItem { Value = "Italy", Text = "Italy" },
new SelectListItem { Value = "Spain", Text = "Spain" },
new SelectListItem { Value = "South Korea", Text = "South Korea" },
new SelectListItem { Value = "Argentina", Text = "Argentina" },
new SelectListItem { Value = "Netherlands", Text = "Netherlands" },
new SelectListItem { Value = "Switzerland", Text = "Switzerland" },
new SelectListItem { Value = "Sweden", Text = "Sweden" },
new SelectListItem { Value = "Norway", Text = "Norway" },
new SelectListItem { Value = "Denmark", Text = "Denmark" },
new SelectListItem { Value = "Saudi Arabia", Text = "Saudi Arabia" },
new SelectListItem { Value = "Singapore", Text = "Singapore" },
new SelectListItem { Value = "New Zealand", Text = "New Zealand" },
new SelectListItem { Value = "Turkey", Text = "Turkey" },
new SelectListItem { Value = "Egypt", Text = "Egypt" },
new SelectListItem { Value = "Nigeria", Text = "Nigeria" },
new SelectListItem { Value = "Vietnam", Text = "Vietnam" },
new SelectListItem { Value = "Thailand", Text = "Thailand" },

            };
            return countries;
        }

        private List<SelectListItem> GetInterests()
        {
            // Replace this with your logic to retrieve interests from the database or other source
            var interests = new List<string>
{
    // Engineering
    "Civil Engineering",
    "Mechanical Engineering",
    "Electrical Engineering",
    "Chemical Engineering",
    "Aerospace Engineering",
    "Biomedical Engineering",
    "Environmental Engineering",
    "Computer Engineering",
    "Materials Science",
    "Industrial Engineering",

    // Mathematics
    "Mathematics and Puzzles",

    // Business
    "Entrepreneurship",
    "Public Speaking",

    // STEM Education
    //"STEM Education",

    // Science
    //"Artificial Intelligence",
    //"Robotics",
    //"Environmental Science",
    //"Space Exploration",
    //"Bioinformatics",

    // Technology
    //"Computer Programming",
    //"Virtual Reality",
    //
    //// Arts and Design
    //"Culinary Arts",
    //"Graphic Design",
    //
    //// Social Sciences
    //"Psychology",
    //"Creative Writing",
    //"Sociology",
    //"History and Archaeology",
    //"Political Science",
    //
    //// Language Learning
    //"Language Learning"
};


            return interests.Select(interest => new SelectListItem
            {
                Value = interest,
                Text = interest
            }).ToList();
        }

        private List<SelectListItem> GetEducationLevels()
        {
            // Replace this with your logic to retrieve education levels from the database or other source
            var educationLevels = new List<SelectListItem>
            {
                new SelectListItem { Value = "Grade 1", Text = "Grade 1" },
                new SelectListItem { Value = "Grade 2", Text = "Grade 2" },
                new SelectListItem { Value = "Grade 3", Text = "Grade 3" },
                new SelectListItem { Value = "Grade 4", Text = "Grade 4" },
                new SelectListItem { Value = "Grade 5", Text = "Grade 5" },
                new SelectListItem { Value = "Grade 6", Text = "Grade 6" },
                new SelectListItem { Value = "Grade 7", Text = "Grade 7" },
                new SelectListItem { Value = "Grade 8", Text = "Grade 8" },
                new SelectListItem { Value = "Grade 9", Text = "Grade 9" },
                new SelectListItem { Value = "Grade 10", Text = "Grade 10" },
                new SelectListItem { Value = "Grade 11", Text = "Grade 11" },
                new SelectListItem { Value = "Grade 12", Text = "Grade 12" },
                new SelectListItem { Value = "Bachelor", Text = "Bachelor's Degree" },
                new SelectListItem { Value = "Master", Text = "Master's Degree" },
                new SelectListItem { Value = "PhD", Text = "PhD" },
            };

            return educationLevels;
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
