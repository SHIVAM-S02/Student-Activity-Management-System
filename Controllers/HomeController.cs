using Student_Activity_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Student_Activity_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Login()
        {
            return RedirectToAction("Login", "Login");
        }

        
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Notice()
        {
            return View();
        }

        public ActionResult AddNotice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNotice(Notice notice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("INSERT INTO Notices (Title, Content, DatePublished) VALUES (@Title, @Content, @DatePublished);", connection))
                        {
                            command.Parameters.AddWithValue("@Title", notice.Title);
                            command.Parameters.AddWithValue("@Content", notice.Content);
                            command.Parameters.AddWithValue("@DatePublished", DateTime.Now);

                            command.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Notification = "Notice added successfully."; 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to add the notice.");
                }
            }

            return RedirectToAction("ViewNotices");
        }

        public ActionResult ViewNotices()
        {
            var notices = new List<Notice>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT * FROM Notices ORDER BY DatePublished DESC;", connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notices.Add(new Notice
                            {
                                ID = (int)reader["ID"],
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                DatePublished = (DateTime)reader["DatePublished"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                ViewBag.ErrorMessage = "An error occurred while retrieving notices.";
            }

            return View(notices);
        }


    }
}