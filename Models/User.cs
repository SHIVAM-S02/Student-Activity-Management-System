using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Student_Activity_Management_System.Models;

namespace Student_Activity_Management_System.Models
{
    public class User
    {

        public int User_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Role_ID { get; set; }

        public string Role_Name = "Student";
        public int? Batch_ID { get; set; }
        public int? Batch_Name { get; set; }
        public string Message { get; set; }


    }
}