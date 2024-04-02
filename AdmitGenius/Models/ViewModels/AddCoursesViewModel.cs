namespace AdmitGenius.Models.ViewModels
{
    public class AddCoursesViewModel
    {
        public string CourseName { get; set; }

        public string CourseDescription { get; set; }

        public IFormFile CourseImage { get; set; }

        public int CourseDuration { get; set; }

        public string CourseDepartment { get; set; }

        public int CourseFees { get; set; }

        public int ApplicationFee { get; set; }

        public DateTime CourseEnrollDeadline { get; set; }

        public string CourseEducationLevel { get; set; }

        public string CourseMinimumRequirements { get; set; }

        public string WhatWillYouLearn { get; set; }

        public string CourseLanguage { get; set; }


        public Guid UniversityId { get; set; }

        public string Hashtags { get; set; }


        public List<string> EducationLevels { get; set; }

        public List<string> MinimumRequirements { get; set; }


        // Minimum requirements for different grading systems
        public string MinimumRequirementsUS { get; set; }
        public string MinimumRequirementsUK { get; set; }
        public float MinimumRequirementsGermany { get; set; }
        public float MinimumRequirementsFrance { get; set; }
        public float MinimumRequirementsJapan { get; set; }
        public float MinimumRequirementsOther { get; set; }



        public List<string> Languages { get; set; }


        public DateTime CourseEnrollDeadlineIntake1 { get; set; }
        public DateTime CourseStartingDateIntake1 { get; set; }


        public DateTime? CourseEnrollDeadlineIntake2 { get; set; }
        public DateTime? CourseStartingDateIntake2 { get; set; }


        public DateTime? CourseEnrollDeadlineIntake3 { get; set; }
        public DateTime? CourseStartingDateIntake3 { get; set; }




        //language exam scores

        public double? IELTS { get; set; }
        public double? TOFEL { get; set; }
        public double? PTE { get; set; }


        public int CourseSemesters { get; set; }

        public int CostOFLiving { get; set; }

        public double? ACT { get; set; }
        public double? SAT { get; set; }
        public double? GRE { get; set; }
    }
}
