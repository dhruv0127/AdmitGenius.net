﻿using System.ComponentModel.DataAnnotations;

namespace AdmitGenius.Models
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }
        public string CompanyTagline { get; set; }
        public string CompanyDescription { get; set; }


        public string CompanyImage { get; set; } 
        public string CompanyLogo { get; set; } 
        public DateTime CompanyRegisterDate { get; set; }



        public string CompanyCountry { get; set; }
        public string CompanyLocation { get; set; }


        public double? logitude { get; set; }
        public double? latitude { get; set; }



        public int TotalClients { get; set; }
        public int TotalProjects { get; set; }
        public int TotalWorkers { get; set; }





        //service and featues
        public string? Feature1 { get; set; }
        public string? Feature2 { get; set; }
        public string? Feature3 { get; set; }
        public string? Feature4 { get; set; }
        public string? Feature5 { get; set; }
        public string? Feature6 { get; set; }


        //service and feature titile
        public string? TitleOfFeature1 { get; set; }
        public string? TitleOfFeature2 { get; set; }
        public string? TitleOfFeature3 { get; set; }
        public string? TitleOfFeature4 { get; set; }
        public string? TitleOfFeature5 { get; set; }
        public string? TitleOfFeature6 { get; set; }



        public string CompanyOfficalEmailAddress { get; set; }
        public string CompanyPassword { get; set; }
        public string RequestStatus { get; set; }



        public string? CompanyWebsite { get; set; }


        // job posting
        public ICollection<Job> Job { get; set; }


        //workers
        public ICollection<Teams> Teams { get; set; }
    }
}

