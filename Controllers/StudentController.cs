using Student_Activity_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Student_Activity_Management_System.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Batches()
        {
            return View();
        }

        public ActionResult Batch_2024()
        {
            List<User> userList = new List<User>();

            // Define the connection string
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetUserByBatchID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                User_ID = (int)reader["User_ID"],
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Batch_ID = reader["Batch_ID"] == DBNull.Value ? null : (int?)reader["Batch_ID"]
                            };

                            userList.Add(user);
                        }
                    }
                }
            }

            return View(userList);
        }

        public ActionResult Batch_2025()
        {
            List<User> userList = new List<User>();


            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetUserByBatch_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                User_ID = (int)reader["User_ID"],
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Batch_ID = reader["Batch_ID"] == DBNull.Value ? null : (int?)reader["Batch_ID"]
                            };

                            userList.Add(user);
                        }
                    }
                }
            }

            return View(userList);
        }

        public ActionResult Batch_2026()
        {
            List<User> userList = new List<User>();

            // Define the connection string
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetUserByBatch_IDs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                User_ID = (int)reader["User_ID"],
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Batch_ID = reader["Batch_ID"] == DBNull.Value ? null : (int?)reader["Batch_ID"]
                            };

                            userList.Add(user);
                        }
                    }
                }
            }

            return View(userList);
        }

        [HttpGet]
        public ActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateStudent(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("CreateStudent", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters

                        cmd.Parameters.AddWithValue("@p_FirstName", user.First_Name);
                        cmd.Parameters.AddWithValue("@p_LastName", user.Last_Name);
                        cmd.Parameters.AddWithValue("@p_Gender", user.Gender);
                        cmd.Parameters.AddWithValue("@p_Email", user.Email);
                        cmd.Parameters.AddWithValue("@p_Password", user.Password);
                        cmd.Parameters.AddWithValue("@p_Batch_ID", user.Batch_ID);

                        cmd.ExecuteNonQuery();
                    }
                }

                ViewBag.Message = "Student created successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Batch_2024", "Student");
        }

        public ActionResult EditStudent(int id)
        {
            // Retrieve the existing admin user's details based on the User_ID
            User student = GetStudentById(id);

            if (student == null)
            {
                return RedirectToAction("Batches"); // Handle admin not found
            }

            return View(student); // Pass the existing admin's details to the view
        }

        [HttpPost]
        public ActionResult EditStudent(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the EditAdmin stored procedure to update the admin's information
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("EditStudent", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@p_UserID", user.User_ID);
                            cmd.Parameters.AddWithValue("@p_FirstName", user.First_Name);
                            cmd.Parameters.AddWithValue("@p_LastName", user.Last_Name);
                            cmd.Parameters.AddWithValue("@p_Gender", user.Gender);
                            cmd.Parameters.AddWithValue("@p_Email", user.Email);
                            cmd.Parameters.AddWithValue("@p_Password", user.Password);
                            cmd.Parameters.AddWithValue("@p_Batch_ID", user.Batch_ID);
                            cmd.Parameters.AddWithValue("@p_Role_ID", user.Role_ID);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Student updated successfully.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Batches", "Student");
        }


        private User GetStudentById(int id)
        {
            User student = null;

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
                            student = new User
                            {
                                User_ID = Convert.ToInt32(reader["User_ID"]),
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Batch_ID = Convert.ToInt32(reader["Batch_ID"]),

                            };
                        }
                    }
                }
            }

            return student;
        }

        [HttpGet]
        public ActionResult DeleteStudent(int id)
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

            return RedirectToAction("Batches", "Student");
        }

        [HttpGet]
        public ActionResult StudentProfile()
        {
            // Get the user ID from the session
            int userId = (int)Session["UserId"];

            // Retrieve the student's profile based on the user ID
            User studentProfile = GetStudentProfile(userId);

            if (Session["UserId"] != null)
            {

                // Create a view model to represent the student's profile data
                User viewModel = new User
                {
                    User_ID = studentProfile.User_ID,
                    First_Name = studentProfile.First_Name,
                    Last_Name = studentProfile.Last_Name,
                    Gender = studentProfile.Gender,
                    Email = studentProfile.Email,
                    // Add more properties from the User model as needed
                };

                // Pass the view model to the view
                return View(viewModel);
            }
            else
            {
                // Handle the case where the student profile was not found
                return RedirectToAction("NotFound", "Error"); // You can create an error handling action for this
            }
        }

        // Define the GetStudentProfile method within the StudentController
        private User GetStudentProfile(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("GetStudentProfile", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@User_ID", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new User
                        {
                            User_ID = reader.GetInt32(reader.GetOrdinal("User_ID")),
                            First_Name = reader.GetString(reader.GetOrdinal("First_Name")),
                            Last_Name = reader.GetString(reader.GetOrdinal("Last_Name")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            // Add more fields as needed
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
            }

            return null; // Return null if the profile is not found or an error occurs
        }

        public ActionResult AcademicData()
        {
            return View();
        }

    }
}