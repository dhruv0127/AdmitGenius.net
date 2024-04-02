using AdmitGenius.Data;
using AdmitGenius.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdmitGenius.Controllers
{
    public class StudentHomeController : Controller
    {
        private readonly AdmitDBContext adminDbContext;
        public StudentHomeController(AdmitDBContext adminDbContext)
        {
            this.adminDbContext = adminDbContext;
        }


        public IActionResult StudentHomePage(string universitySearchField, string courseSearchField, string countryFilter)
        {
            var approvedUniversities = adminDbContext.Universities
                .Where(u => u.RequestStatus == "Approved").ToList(); 

            if (!string.IsNullOrEmpty(universitySearchField))
            {
                approvedUniversities = approvedUniversities
                    .Where(u => u.UniversityName.Contains(universitySearchField, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(countryFilter))
            {
                approvedUniversities = approvedUniversities
                    .Where(u => u.UniversityCountry.Equals(countryFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var allCourses = adminDbContext.Courses.ToList();

            if (!string.IsNullOrEmpty(courseSearchField))
            {
                allCourses = allCourses
                    .Where(c => c.CourseName.Contains(courseSearchField, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var viewModel = new StudentHomePageViewModel
            {
                Universities = approvedUniversities,
                Courses = allCourses
            };

            return View(viewModel);
        }



        public async Task<IActionResult> SearchCourses(string searchTerm, List<string> selectedHashtags)
        {
            // Query courses based on the search term and selected hashtags
            var courses = await adminDbContext.Courses
                .Include(c => c.University)
                .Where(c => c.CourseName.Contains(searchTerm) ||
                            c.Hashtags.Any(h => selectedHashtags.Contains(h.Name)))
                .ToListAsync();

            // You may need to adjust the above query based on your exact requirements

            // Retrieve only approved universities
            var approvedUniversities = adminDbContext.Universities
                .Where(u => u.RequestStatus == "Approved")
                .ToList();

            // Create an instance of the ViewModel and populate its properties
            var viewModel = new StudentHomePageViewModel
            {
                Universities = approvedUniversities,
                Courses = courses
            };

            // Pass the ViewModel to the view
            return View("StudentHomePage", viewModel);
        }

    }
}

