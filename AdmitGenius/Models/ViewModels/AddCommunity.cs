namespace AdmitGenius.Models.ViewModels
{
    public class AddCommunity
    {
        public string CommunityName { get; set; }

        public string CommunityDescription { get; set; }
        public IFormFile? CommunityImage { get; set; }


        public string? LastMessage { get; set; }



        public DateTime CommunityCreationDate { get; set; }


        public Guid GroupAdminId { get; set; }
        public string GroupAdminName { get; set; }


    }
}
