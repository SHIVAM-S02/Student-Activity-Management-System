using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Student_Activity_Management_System.Models
{
    public class AcademicDetails
    {
        public int ID { get; set; }
        public int User_ID { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public double CGPA { get; set; }
        public int ClosedBacklog { get; set; }
        public int LiveBacklog { get; set; }
        public string FileUploadPath { get; set; }
    }
}