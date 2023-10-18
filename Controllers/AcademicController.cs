using Student_Activity_Management_System.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Student_Activity_Management_System.Controllers
{
    public class AcademicController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public ActionResult EducationalDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EducationalDetails(EducationalDetail info, HttpPostedFileBase FileUpload)
        {
            if (ModelState.IsValid)
            {

                string FileUploadPath = null;
                if (FileUpload != null && FileUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/PDFs/EducationalData"), fileName);
                    FileUpload.SaveAs(filePath);
                    FileUploadPath = "~/Content/PDFs/EducationalData/" + fileName;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                if (UploadEducationalDetails(info.YearOfPassing, info.Degree, info.Board, info.Percentage, FileUploadPath, userId))
                {
                    return RedirectToAction("ViewEducationalDetails");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to upload the educational information.");
                }
            }

            return View();
        }

        private bool UploadEducationalDetails(int yearOfPassing, string degree, string board, double percentage, string FileUploadPath, int userId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("InsertEducationalDetail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@YearOfPassing", yearOfPassing);
                        command.Parameters.AddWithValue("@Degree", degree);
                        command.Parameters.AddWithValue("@Board", board);
                        command.Parameters.AddWithValue("@Percentage", percentage);
                        command.Parameters.AddWithValue("@FileUploadPath", FileUploadPath);
                        command.Parameters.AddWithValue("@User_ID", userId);

                        command.ExecuteNonQuery();
                    }
                }

                return true; // Successful upload
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                return false; // Upload failed
            }
        }


        public ActionResult ViewEducationalDetails()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            List<EducationalDetail> educationalDetails = GetEducationalDetailsByUserId(userId);
            return View(educationalDetails);
        }

        private List<EducationalDetail> GetEducationalDetailsByUserId(int userId)
        {
            List<EducationalDetail> educationalDetails = new List<EducationalDetail>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("GetEducationalDetailsByUserId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@User_ID", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var educationalDetail = new EducationalDetail
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Degree = reader["Degree"].ToString(), // Map Degree property
                                    Board = reader["Board"].ToString(),   // Map Board property
                                    Percentage = Convert.ToDouble(reader["Percentage"]), // Map Percentage property
                                    YearOfPassing = Convert.ToInt32(reader["YearOfPassing"]), // Map YearOfPassing property
                                    FileUploadPath = reader["FileUploadPath"].ToString() // Map FileUpload property

                                };
                                educationalDetails.Add(educationalDetail);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
            }

            return educationalDetails;
        }

        [HttpPost]
        public ActionResult DeleteEducationalDetail(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("DeleteEducationalDetail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the ID parameter for the stored procedure
                        command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed, e.g., logging or displaying an error message
                ViewBag.ErrorMessage = "An error occurred while deleting educational detail.";
            }

            // After successful deletion or in case of an error, redirect to the "ViewEducationalDetails" action to refresh the data
            return RedirectToAction("ViewEducationalDetails");
        }

        public FileResult ViewCertificate(string FileUploadPath)
        {
            string filePath = Server.MapPath(FileUploadPath);
            return File(filePath, "application/pdf");
        }

        public FileResult DownloadCertificate(string FileUploadPath)
        {
            string filePath = Server.MapPath(FileUploadPath);
            string fileName = Path.GetFileName(filePath);
            return File(filePath, "application/pdf", fileName);
        }


        /*0000000000000000000000000000000000000000000000*/




        public ActionResult AddAcademicDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAcademicDetails(AcademicDetails academicDetails, HttpPostedFileBase FileUpload)
        {
            if (ModelState.IsValid)
            {
                string FileUploadPath = null;
                if (FileUpload != null && FileUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/PDFs/AcademicData"), fileName);
                    FileUpload.SaveAs(filePath);
                    FileUploadPath = "~/Content/PDFs/AcademicData/" + fileName;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                if (UploadAcademicDetails(userId, academicDetails.Year, academicDetails.Semester, academicDetails.CGPA, academicDetails.ClosedBacklog, academicDetails.LiveBacklog, FileUploadPath))
                {
                    return RedirectToAction("ViewAcademicDetails");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to upload the academic information.");
                }
            }

            return View();
        }

        private bool UploadAcademicDetails(int User_ID, int Year, string Semester, double CGPA, int ClosedBacklog, int LiveBacklog, string FileUploadPath)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("InsertAcademicDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@User_ID", User_ID);
                        command.Parameters.AddWithValue("@Year", Year);
                        command.Parameters.AddWithValue("@Semester", Semester);
                        command.Parameters.AddWithValue("@CGPA", CGPA);
                        command.Parameters.AddWithValue("@ClosedBacklog", ClosedBacklog);
                        command.Parameters.AddWithValue("@LiveBacklog", LiveBacklog);
                        command.Parameters.AddWithValue("@FileUploadPath", FileUploadPath);

                        command.ExecuteNonQuery();
                    }
                }

                return true; // Successful upload
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                return false; // Upload failed
            }
        }

    }
}
