using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Experiences
    {
        [Key]

        public Guid ExpId { get; set; }


        public string ExpName { get; set; }
        public string ExpPostName { get; set; }
        public string? ExpComapnyName { get; set; }



        public string ExpStartDate { get; set; }
        public string? ExpEndDate { get; set; }


        public string? Responsibilities { get; set; }




        //foreign key
        [ForeignKey("Users")]
        public Guid UserId { get; set; }

        public virtual Users Users { get; set; }


        [ForeignKey("counselors")]
        public Guid? CounselorId { get; set; }

        public virtual Counselors counselors { get; set; }


    }
}
