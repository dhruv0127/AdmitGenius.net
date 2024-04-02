using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Scholarship
    {
        [Key]
        public Guid ScholrshipId { get; set; }

        public string ScholarshipName { get; set; }

        public string ScholarshipDescription { get; set; }

        public string? ScholarshipPrice { get; set; }

        public string? ScholrshipImage {  get; set; }


        public string RequestStatus { get; set; }


        [DataType(DataType.Date)] // Annotation for specifying the data type
        public DateTime? ScholarshipPostedDate { get; set; }

        public string ScholarshipLink { get; set; }
    }
}
