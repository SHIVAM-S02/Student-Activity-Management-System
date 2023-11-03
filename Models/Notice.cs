using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Student_Activity_Management_System.Models
{
    public class Notice
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
    }
}