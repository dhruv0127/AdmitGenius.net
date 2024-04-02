using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class S3FileDetails
    {
        [Key]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string S3Url { get; set; }
    }
}
