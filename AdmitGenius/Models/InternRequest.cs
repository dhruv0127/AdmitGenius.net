using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class InternRequest
    {
        [Key]
        public Guid IrID { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string Status { get; set; }




        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual Users User { get; set; }





        public Guid  ccid{ get; set; }


        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public virtual Job Job { get; set; }

    }
}
