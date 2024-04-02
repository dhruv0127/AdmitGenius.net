using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class StudyDestinationCountry
    {
        [Key]
        public int SDCId { get; set; }

        public string CountryName { get; set; }
    }
}
