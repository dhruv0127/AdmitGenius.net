namespace AdmitGenius.Models.ViewModels
{
    public class AddCounselorViewModel
    {
        public string? CounselorFirstName { get; set; }
        public string? CounselorLastName { get; set; } //new


        public string CounselorEmail { get; set; }
        public string CounselorPassword { get; set; }


        public string CounselorGender { get; set; }//new
        public DateTime CounselorBirthday { get; set; }//new


        public string CounselorBio { get; set; } //new
        public int? CounselorPhone { get; set; }


        public string? CounselorCountry { get; set; }
        public string? CounselorFullLocation { get; set; }


        public string RequestStatus { get; set; }

        public IFormFile CounselorProfilePhoto { get; set; }
        public int TotalPlacedStudent { get; set; }  //new


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



        //social links

        public string? CounselorInsta { get; set; } //new
        public string? CounselorFb { get; set; } //new
        public string? CounselorX { get; set; } //new


        //Work
        public string? CurrentComapny { get; set; }  //new



    }
}
