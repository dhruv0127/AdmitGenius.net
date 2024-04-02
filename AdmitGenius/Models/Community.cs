using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Community
    {
        [Key]
        public Guid CommunityId { get; set; }
        public string CommunityName { get; set;}

        public string CommunityDescription { get; set;}
        public string? CommunityImage { get; set;}

        
        public string? LastMessage { get; set; }



        public DateTime CommunityCreationDate { get; set; }


        public Guid GroupAdminId { get; set; }
        public string GroupAdminName{ get; set; }


        public ICollection<CommunityMember> CommunityMember { get; set; }

    }
}
