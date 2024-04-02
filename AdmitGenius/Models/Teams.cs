using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Teams
    {
        [Key]
        public Guid TeamId { get; set; }

        public string TeamName { get; set; }

        public string TeamPost { get; set;}

        public string TeamProfile { get; set;}



        [ForeignKey("Company")]
        public Guid? CompanyId { get; set; }

        public virtual Company Company { get; set; }



        [ForeignKey("Universities")]
        public Guid? UniversityId { get; set; }

        public virtual Universities Universities { get; set; }
    }
}
