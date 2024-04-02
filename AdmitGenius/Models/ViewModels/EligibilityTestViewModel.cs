using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AdmitGenius.Models.ViewModels
{
    public class EligibilityTestViewModel
    {
        public List<SelectListItem> AvailableCountries { get; set; }
        public List<string> StudyDestinationCountries { get; set; } // Change here
        public int Budget { get; set; }

        public List<string> Interests { get; set; }
        public List<SelectListItem> AvailableInterests { get; set; }
        public List<string> SelectedInterests { get; set; }

        public List<SelectListItem> AvailableEducationLevels { get; set; }
        public string HighestEducationLevel { get; set; }
        public string LastEducationGrade { get; set; }

        public string GradingSystem { get; set; }


        public double? ACT { get; set; }
        public double? SAT { get; set; }
        public double? GRE { get; set; }

        public double? IELTS { get; set; }
        public double? TOFEL { get; set; }
        public double? PTE { get; set; }

        public EligibilityTestViewModel()
        {
            // Initialize lists here if needed
            AvailableCountries = new List<SelectListItem>();
            AvailableInterests = new List<SelectListItem>();
            AvailableEducationLevels = new List<SelectListItem>();
            SelectedInterests = new List<string>();
        }
    }
}
