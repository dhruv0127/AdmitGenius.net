using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class EligibilityTest
    {
        [Key]
        public Guid EligibilityTestId { get; set; }
        public List<Interest> Interests { get; set; }
        public List<StudyDestinationCountry> StudyDestinationCountries { get; set; }
        public int Budget { get; set; }
        public string HighestEducationLevel { get; set; }
        public string GradingSystem { get; set; }



        // Minimum requirements for different grading systems
        public string? GradesInUS { get; set; }
        public string? GradesInUK { get; set; }
        public float?  GradesInGermany { get; set; }
        public float?  GradesInFrance { get; set; }
        public float?  GradesInJapan { get; set; }
        public float?  GradesInOther { get; set; }


        public double? ACT { get; set; }
        public double? SAT { get; set; }
        public double? GRE { get; set; }

        public double? IELTS { get; set; }
        public double? TOFEL { get; set; }
        public double? PTE { get; set; }


        // Establish the relationship with Users table
        public virtual Users User { get; set; }

        
    }
}
