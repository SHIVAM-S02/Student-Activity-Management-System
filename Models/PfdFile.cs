using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Student_Activity_Management_System.Models
{
    public class PdfFile
    {
        /*public int PdfId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }*/

        public int File_ID { get; set; }
        public string FileName { get; set; }
        public int Marks { get; set; }
        public string Degree { get; set; }
        public string Board { get; set; }
        public double Percentage { get; set; }
        public string Year_of_Passing { get; set; }
        public byte[] FileData { get; set; }
        public int User_ID { get; set; }
    }

}