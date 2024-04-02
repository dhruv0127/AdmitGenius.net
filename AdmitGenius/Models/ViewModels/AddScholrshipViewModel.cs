namespace AdmitGenius.Models.ViewModels
{
    public class AddScholrshipViewModel
    {
        public string ScholarshipName { get; set; }

        public string ScholarshipDescription { get; set; }

        public string? ScholarshipPrice { get; set; }

        public IFormFile? ScholrshipImage { get; set; }


        public string RequestStatus { get; set; }

        public DateOnly? ScholarshipPostedDate { get; set; }

        public string ScholarshipLink { get; set; }
    }
}
