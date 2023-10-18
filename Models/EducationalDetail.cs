using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Student_Activity_Management_System.Models
{
    public class EducationalDetail
    {
        public int ID { get; set; }

        [Required]
        public int YearOfPassing { get; set; }

        [Required]
        public string Degree { get; set; }

        [Required]
        public string Board { get; set; }

        [Required]
        [Range(0, 100)]
        public double Percentage { get; set; }

        public string FileUploadPath { get; set; }
    }
}
