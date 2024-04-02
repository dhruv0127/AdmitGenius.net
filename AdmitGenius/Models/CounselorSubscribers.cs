using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class CounselorSubscribers
    {
        [Key]
        public Guid CSID { get; set; }



        public Guid UserId { get; set; }
        public virtual Users User { get; set; }



        [ForeignKey("Counselors")]
        public Guid CounselorId { get; set; }

        public virtual Counselors Counselors { get; set; }



        public string GroupName { get; set; }


        public string status { get; set; }




        public string LastMessage { get; set; }





    }
}
