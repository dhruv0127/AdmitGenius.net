// Courses.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Courses
    {
        [Key]
        public Guid CourseId { get; set; }
        public String CourseName { get; set; }
        public String CourseDescription { get; set; }
        public String CourseImage { get; set; }
        public int CourseDuration { get; set; }
        public int CourseSemesters { get; set; }


        //fees
        public int CourseFees { get; set; } 
        public int ApplicationFee { get; set; }
        public int CostOFLiving { get; set; }



        public string CourseEducationLevel { get; set; }
        public string CourseMinimumRequirements { get; set; }

        
        // Minimum requirements for different grading systems
        public string MinimumRequirementsUS { get; set; }
        public string MinimumRequirementsUK { get; set; }
        public float MinimumRequirementsGermany { get; set; }
        public float MinimumRequirementsFrance { get; set; }
        public float MinimumRequirementsJapan { get; set; }
        public float MinimumRequirementsOther { get; set; }







        public string WhatWillYouLearn {  get; set; }   


        public string CourseLanguage { get; set; }



        [ForeignKey("University")]
        public Guid UniversityId { get; set; }        

        public virtual Universities University { get; set; }



        public string CourseDepartment { get; set; }



        //language exam scores

        public double? IELTS { get; set; }
        public double? TOFEL { get; set; }
        public double? PTE { get; set; }


        
        public double? ACT { get; set; }
        public double? SAT { get; set; }
        public double? GRE { get; set; }



        //intakes
        public DateTime CourseEnrollDeadlineIntake1 { get; set; }
        public DateTime CourseStartingDateIntake1 { get; set; }


        public DateTime? CourseEnrollDeadlineIntake2 { get; set; }
        public DateTime? CourseStartingDateIntake2 { get; set; }


        public DateTime? CourseEnrollDeadlineIntake3 { get; set; }
        public DateTime? CourseStartingDateIntake3 { get; set; }




        


        public virtual ICollection<Hashtag> Hashtags { get; set; } = new List<Hashtag>();
    }
}
