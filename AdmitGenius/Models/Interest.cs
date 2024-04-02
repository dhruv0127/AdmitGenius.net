using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Interest
    {
        [Key]
        public Guid InterestId { get; set; }
        public string Name { get; set; }
    }
}
