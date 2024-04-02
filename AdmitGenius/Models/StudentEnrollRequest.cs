using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class StudentEnrollRequest
    {
        [Key]
        public Guid SerID { get; set; }
        public Guid Universityiid { get; set; }
        public Guid StudentID { get; set; }
        public Guid CourseID { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string Status { get; set; }
        public String? CounselorEmailID { get; set; }


        [ForeignKey("StudentID")]
        public virtual Users Student { get; set; }    

        [ForeignKey("CourseID")]
        public virtual Courses Course { get; set; }


    }
}
