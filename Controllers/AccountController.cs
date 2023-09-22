using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Student_Activity_Management_System.Models;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Student_Activity_Management_System.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand("sp_UserSignUp", connection);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@First_Name", model.First_Name);
                        cmd.Parameters.AddWithValue("@Last_Name", model.Last_Name);
                        cmd.Parameters.AddWithValue("@Gender", model.Gender);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Password", model.Password);
                        cmd.Parameters.AddWithValue("@Role_Name", "Student");
                        cmd.Parameters.AddWithValue("@Batch_Name", model.Batch_Name);

                        cmd.ExecuteNonQuery();

                        // Redirect to a success page or login page
                        return RedirectToAction("Login", "Account");
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand("UserLogin", connection);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            var user = new User
                            {
                                User_ID = reader.GetInt32(0),
                                Role_Name = reader.GetString(1)
                            };

                            // Set authentication cookie
                            FormsAuthentication.SetAuthCookie(model.Email, false);

                            Session["UserRole"] = user.Role_Name;
                            Session["UserId"] = user.User_ID; // Store the user ID in the session

                            if (user.Role_Name == "Admin")
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            else if (user.Role_Name == "Student")
                            {
                                return RedirectToAction("Dashboard", "Student");
                            }
                            else if (user.Role_Name == "Teacher")
                            {
                                return RedirectToAction("Dashboard", "Teacher");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid email or password.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            return View(model);
        }



        public ActionResult Logout()
        {
            // Clear session variables
            Session.Clear();

            // Sign out the user
            FormsAuthentication.SignOut();

            // Redirect to the home page or login page
            return RedirectToAction("Home", "Home");
        }



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
                            Password = reader.GetString(reader.GetOrdinal("Password"))
                            /*Batch_Name = reader.GetString(reader.GetOrdinal("Batch_Name"))*/
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



    }
}
