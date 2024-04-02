using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Education
    {
        [Key]
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }

        public DateTime? EducationStartDate { get; set;}
        public DateTime? EducationEndDate { get; set;}

        public string SchoolName { get; set; }


        public float  Percentage { get; set; }
        public string SchoolCity { get; set; }






        //foregin key


        [ForeignKey("users")]
        public Guid UserId { get; set; }

        public virtual Users users { get; set; }


        [ForeignKey("counselors")]
        public Guid? CounselorId { get; set; }

        public virtual Counselors counselors { get; set; }

    }
}
