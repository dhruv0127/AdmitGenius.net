using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Users
    {
        [Key]
        public Guid UserId { get; set; }
        public string? UserName { get; set; }

        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string UserPassword { get; set; }
        public long? UserPhone { get; set; }
        public string? UserCountry { get; set; }
        public string? UserFullLocation { get; set; }
        public string? UserGraduation { get; set; }
        public string UserRole { get; set; }

        public string? UserProfile { get; set; }     

        
        public ICollection<StudentEnrollRequest> Enrollments { get; set; }
        public ICollection<InternRequest> InternRequest { get; set; }
       
        
       

        //social Links 
        
        public string? LinkedIn { get; set; }

        public string? Github { get; set; }

        public string? FaceBook { get; set; }

        public string? Twitter { get; set; }
        
        
        
        
        public string? Skills { get; set; }
        public string? resumeFileString { get; set; }



        public string? resumePrefrence { get; set; }
        


         //new   
        public string? UserBio {  get; set; }

        
        // Add a foreign key reference to the eligibility information
        public Guid? EligibilityTestId { get; set; } 

        public EligibilityTest EligibilityTest { get; set; }



        public List<CounselorSubscribers> CounselorSubscribers { get; set; }

        public ICollection<Experiences> Experiences { get; set; }

        public ICollection<Education> Education { get; set; }

        public ICollection<Projects> Projects { get; set; }



    }
}
