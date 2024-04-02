using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models.ViewModels
{
    public class AddUniversityViewModel
    {
        public string UniversityName { get; set; }
        public string UniversityTagline { get; set; }
        public string UniversityDescription { get; set; }

        public IFormFile UniversityImageFile { get; set; }
        public List<IFormFile> AdditionalImagesFiles { get; set; }

        public string UniversityCountry { get; set; }
        public string UniversityLocation { get; set; }
        public DateTime UniversityRegisterDate { get; set; }
        public string UniversityOfficalEmailAddress { get; set; }
        public string UniversityPassword { get; set; }
        public string RequestStatus { get; set; }
        public string UniversityType { get; set; }


        //leter of acceptanace
        public string LetterofAcceptanceJtoA { get; set; }
        public string LetterofAcceptanceMtoA { get; set; }
        public string LetterofAcceptanceStoD { get; set; }


        //features
        public string? Feature1 { get; set; }
        public string? Feature2 { get; set; }
        public string? Feature3 { get; set; }
        public string? Feature4 { get; set; }
        public string? Feature5 { get; set; }


        //logitude

        public double? logitude { get; set; }
        public double? latitude { get; set; }

    }
}
