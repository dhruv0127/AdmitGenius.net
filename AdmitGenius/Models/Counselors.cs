using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Counselors
    {
        [Key]
        public Guid CounselorId { get; set; }
        public string? CounselorFirstName { get; set; }
        public string? CounselorLastName { get; set; } //new
        public string CounselorEmail { get; set; }
        public string CounselorPassword { get; set; }

        //new
        public string CounselorGender { get; set; }
        public DateTime CounselorBirthday { get; set; }

        
        public string CounselorBio { get; set; }

        public long? CounselorPhone { get; set; }
        public string? CounselorCountry { get; set; }
        public string? CounselorFullLocation { get; set; }
        public string RequestStatus { get; set; }

        public string CounselorProfilePicture { get; set; }

        public int TotalPlacedStudent { get; set; }



        //graduation

        public string? CounselorDegree { get; set; }
        public string? CounselorUniversity { get; set; }
        public string? CounselorGraduationYear { get; set; }


        //availability

        public string WorkingHours { get; set; }
        public string DaysAvailable { get; set; }


        //lanuage

        public string MainLanguage { get; set; }

        public string? SecondaryLanguage { get; set; }


        // Counselor Fees
        public string CounselorFees { get; set; }



        //Work
        public string? CurrentComapny { get; set; }



        //social links

        public string? CounselorInsta { get; set; }
        public string? CounselorFb { get; set; }
        public string? CounselorX { get; set; }






        public ICollection<Experiences> Experiences { get; set; }

        public ICollection<Education> Education { get; set; }


        public List<CounselorSubscribers> CounselorSubscribers { get; set; }



    }
}
