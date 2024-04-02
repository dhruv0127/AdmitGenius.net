// Universities.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class Universities
    {
        public Universities()
        {
            Courses = new List<Courses>();
        }

        [Key]
        public Guid UniversityId { get; set; }
        public string UniversityName { get; set; }
        public string UniversityTagline { get; set; }
        public string UniversityDescription { get; set; }
        public string UniversityImage { get; set; }
        public string UniversityCountry { get; set; }
        public string UniversityLocation { get; set; }
        public DateTime UniversityRegisterDate { get; set; }
        public string UniversityOfficalEmailAddress { get; set; }
        public string UniversityPassword { get; set; }
        public string RequestStatus { get; set; }


        public string? UniversityImage2 { get; set; }
        public string? UniversityImage3 { get; set; }
        public string? UniversityImage4 { get; set; }
        public string? UniversityImage5 { get; set; }
        public string? UniversityImage6 { get; set; }


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




        public List<Courses> Courses { get; set; }
        public ICollection<Teams> Teams { get; set; }

    }
}
