namespace AdmitGenius.ViewModels
{
    public class CounselorViewModel
    {
        public Guid CounselorId { get; set; }
        public string CounselorFirstName { get; set; }
        public string CounselorEmail { get; set; }
        public string CounselorPassword { get; set; }
        public string CounselorBio { get; set; }
        public int? CounselorPhone { get; set; }
        public string CounselorCountry { get; set; }
        public string CounselorFullLocation { get; set; }
        public string RequestStatus { get; set; }
        public string CounselorProfilePicture { get; set; }


        public IFormFile CounselorProfilePhoto { get; set; }


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
        public decimal CounselorFees { get; set; }


        //social links

        public string? CounselorInsta { get; set; }
        public string? CounselorFb { get; set; }
        public string? CounselorX { get; set; }


        //Work
        public string? CurrentComapny { get; set; }
    }
}
