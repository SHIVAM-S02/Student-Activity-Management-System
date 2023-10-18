using Student_Activity_Management_System.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Data;

public class PdfController : Controller
{
    private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


    public ActionResult Submit()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Submit(NonAcademicInfo info, HttpPostedFileBase certificate)
    {
        if (ModelState.IsValid)
        {
            string certificatePath = null;
            if (certificate != null && certificate.ContentLength > 0)
            {
                string fileName = Path.GetFileName(certificate.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Content/PDFs/NonAcademicData"), fileName);
                certificate.SaveAs(filePath);
                certificatePath = "~/Content/PDFs/NonAcademicData/" + fileName; // Corrected path
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            if (UploadNonAcademicInfo(info.Year, info.Category, info.Activity, certificatePath, userId))
            {
                return RedirectToAction("NonAcademicData");
            }
            else
            {
                ModelState.AddModelError("", "Failed to upload the non-academic information.");
            }
        }

        return View();
    }


    private bool UploadNonAcademicInfo(string year, string category, string activity, string certificatePath, int userId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("InsertNonAcademicInfo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Activity", activity);
                    command.Parameters.AddWithValue("@CertificatePath", certificatePath);
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


    public ActionResult NonAcademicData()
    {
        List<NonAcademicInfo> nonAcademicData = new List<NonAcademicInfo>();

        try
        {
            // Ensure that 'connectionString' is defined and contains the correct connection string
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("GetNonAcademicInfo", connection))
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Provide the '@User_ID' parameter
                    cmd.Parameters.Add(new SqlParameter("@User_ID", userId));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NonAcademicInfo info = new NonAcademicInfo
                            {
                                ID = (int)reader["ID"],
                                Year = (string)reader["Year"],
                                Category = (string)reader["Category"],
                                Activity = (string)reader["Activity"],
                                CertificatePath = (string)reader["CertificatePath"]
                            };

                            nonAcademicData.Add(info);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed (e.g., logging or displaying an error message)
            ViewBag.ErrorMessage = "An error occurred while retrieving non-academic data.";
        }

        return View(nonAcademicData);
    }
    
    [HttpPost]
    public ActionResult DeleteNonAcademicInfo(int id)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("DeleteNonAcademicInfo", connection))
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
            ViewBag.ErrorMessage = "An error occurred while deleting non-academic data.";
        }

        // After successful deletion or in case of an error, redirect to the "NonAcademicData" action to refresh the data
        return RedirectToAction("NonAcademicData");
    }

    public FileResult ViewCertificate(string certificatePath)
    {
        string filePath = Server.MapPath(certificatePath);
        return File(filePath, "application/pdf");
    }

    public FileResult DownloadCertificate(string certificatePath)
    {
        string filePath = Server.MapPath(certificatePath);
        string fileName = Path.GetFileName(filePath);
        return File(filePath, "application/pdf", fileName);
    }





}
