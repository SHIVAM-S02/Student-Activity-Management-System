using Student_Activity_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Student_Activity_Management_System.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.Mvc;
using System;

namespace Student_Activity_Management_System.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Teachers()
        {
            List<User> teachers = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetAllTeachers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User teacher = new User
                            {
                                User_ID = Convert.ToInt32(reader["User_ID"]),
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                /*Role_ID = Convert.ToInt32(reader["Role_ID"]),*/
                            };
                            teachers.Add(teacher);
                        }
                    }
                }
            }

            return View(teachers);
        }

        [HttpGet]
        public ActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTeacher(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("CreateTeacher", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@p_FirstName", user.First_Name);
                        cmd.Parameters.AddWithValue("@p_LastName", user.Last_Name);
                        cmd.Parameters.AddWithValue("@p_Gender", user.Gender);
                        cmd.Parameters.AddWithValue("@p_Email", user.Email);
                        cmd.Parameters.AddWithValue("@p_Password", user.Password);

                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Message = "Teacher created successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Teachers", "Teacher");
        }

        public ActionResult EditTeacher(int id)
        {
            // Retrieve the existing admin user's details based on the User_ID
            User teacher = GetTeacherById(id);

            if (teacher == null)
            {
                return RedirectToAction("Teachers"); // Handle admin not found
            }

            return View(teacher); // Pass the existing admin's details to the view
        }

        [HttpPost]
        public ActionResult EditTeacher(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the EditAdmin stored procedure to update the admin's information
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("EditTeacher", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@p_UserID", user.User_ID);
                            cmd.Parameters.AddWithValue("@p_FirstName", user.First_Name);
                            cmd.Parameters.AddWithValue("@p_LastName", user.Last_Name);
                            cmd.Parameters.AddWithValue("@p_Gender", user.Gender);
                            cmd.Parameters.AddWithValue("@p_Email", user.Email);
                            cmd.Parameters.AddWithValue("@p_Password", user.Password);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Teacher updated successfully.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Teachers", "Teacher");
        }


        private User GetTeacherById(int id)
        {
            User teacher = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM users WHERE User_ID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            teacher = new User
                            {
                                User_ID = Convert.ToInt32(reader["User_ID"]),
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),

                            };
                        }
                    }
                }
            }

            return teacher;
        }

        [HttpGet]
        public ActionResult DeleteTeacher(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM users WHERE User_ID = @UserID";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserID", id);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Teachers", "Teacher");
        }

    }
}