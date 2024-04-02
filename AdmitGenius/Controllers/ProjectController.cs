using AdmitGenius.Data;
using AdmitGenius.Models;
using AdmitGenius.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AdmitGenius.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AdmitDBContext dbContext;

        public ProjectController(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }



        [HttpGet]
        public IActionResult AddProject()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProject(addProjectViewModel addProject)
        {

            Guid userId = GetUserIdFromSession();

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Users");
            }


            var project = new Projects()
            {
                ProjectId = Guid.NewGuid(),

                ProjectName = addProject.ProjectName,
                ProjectDescription = addProject.ProjectDescription,
                GithubRepoUrl = addProject.GithubRepoUrl,

                UserId = userId
            };


            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "StudentProfile");
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
