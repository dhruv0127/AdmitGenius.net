using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Job
    {
        [Key]
        public Guid JobId { get; set; }

        public string JobName { get; set; }
        public string JobDesignation { get; set; }

        public string JobDescription { get; set; }


        public DateTime JobPostedTime { get; set; } 


        public string JobEmployeeType { get; set; }


        public string JobLocation { get; set; }



        public string JobMinSalary { get; set; }
        public string JobMaxSalary { get;set; }



        public string JobMiniQualification { get; set; }




        public string JobResponsibility { get; set; }

        public string JobRequirements { get; set; }





        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }


    }
}
