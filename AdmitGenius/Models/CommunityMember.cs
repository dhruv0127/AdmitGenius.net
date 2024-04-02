using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class CommunityMember
    {
        [Key]

        public Guid CMID{ get; set; }



        public Guid UserId { get; set; }
        public virtual Users User { get; set; }



        public Guid CommunityId { get; set; }
        public virtual Community Coomunity { get; set; }



        public string status { get; set; }
    }
}
