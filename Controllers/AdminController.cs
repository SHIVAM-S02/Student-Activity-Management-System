using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Student_Activity_Management_System.Models;

namespace Student_Activity_Management_System.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Admins()
        {
            List<User> admins = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetAllAdmins", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User admin = new User
                            {
                                User_ID = Convert.ToInt32(reader["User_ID"]),
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                /*Role_ID = Convert.ToInt32(reader["Role_ID"]),*/
                            };
                            admins.Add(admin);
                        }
                    }
                }
            }

            return View(admins);
        }

        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("CreateAdmin", connection))
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

                ViewBag.Message = "Admin created successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Admins", "Admin");
        }

        public ActionResult EditAdmin(int id)
        {
            // Retrieve the existing admin user's details based on the User_ID
            User admin = GetAdminById(id);

            if (admin == null)
            {
                return RedirectToAction("Admins"); // Handle admin not found
            }

            return View(admin); // Pass the existing admin's details to the view
        }

        [HttpPost]
        public ActionResult EditAdmin(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Call the EditAdmin stored procedure to update the admin's information
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("EditAdmin", connection))
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

                    ViewBag.Message = "Admin updated successfully.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToAction("Admins", "Admin");
        }


        private User GetAdminById(int id)
        {
            User admin = null;

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
                            admin = new User
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

            return admin;
        }

        [HttpGet]
        public ActionResult DeleteAdmin(int id)
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

            return RedirectToAction("Admins", "Admin");
        }


        public ActionResult Details(int id)
        {
            User adminUser = null;

            try
            {
                // Create a connection to your database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Define the SQL query to retrieve user details
                    string query = "SELECT * FROM Users WHERE User_ID = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Add the parameter for the user ID
                        cmd.Parameters.AddWithValue("@UserId", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a User object and populate it with data from the SqlDataReader
                                adminUser = new User
                                {
                                    User_ID = reader.GetInt32(reader.GetOrdinal("User_ID")),
                                    First_Name = reader.GetString(reader.GetOrdinal("First_Name")),
                                    Last_Name = reader.GetString(reader.GetOrdinal("Last_Name")),
                                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    // Add more fields as needed
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                ModelState.AddModelError("", "An error occurred while processing your request.");
            }

            if (adminUser == null)
            {
                return HttpNotFound(); // Handle the case where the user is not found.
            }

            return View(adminUser); // Pass the user details to the "Details" view.
        }

        // Other actions and code...
    }
}