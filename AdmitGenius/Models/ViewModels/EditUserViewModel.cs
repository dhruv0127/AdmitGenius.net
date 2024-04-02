namespace AdmitGenius.Models.ViewModels
{
    public class EditUserViewModel
    {
        
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public long? UserPhone { get; set; }
        public string UserCountry { get; set; }
        public string UserFullLocation { get; set; }
        public string UserGraduation { get; set; }


        public IFormFile UserProfile { get; set; }
        public IFormFile resumeFileString { get; set; }

        public string? resumePrefrence { get; set; }



        //social Links 

        public string? LinkedIn { get; set; }

        public string? Github  { get; set; }

        public string? FaceBook { get; set; }

        public string? Twitter { get; set; }



        public string? Skills { get; set; }




        public string? UserBio { get; set; }
    }
}
