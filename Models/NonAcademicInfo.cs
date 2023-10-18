using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Student_Activity_Management_System.Models
{
    public class NonAcademicInfo
    {
        public int ID { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }
        public string Activity { get; set; }
        public string CertificatePath { get; set; }
        public int User_ID { get; set; }
    }

}