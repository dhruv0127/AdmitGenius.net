using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Projects
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }


        public string? GithubRepoUrl { get; set; }




        [ForeignKey("users")]
        public Guid UserId { get; set; }

        public virtual Users users { get; set; }
    }
}
