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

    public ActionResult Upload()
    {
        if (Session["UserRole"]?.ToString() == "Student")
        {
            return View();
        }
        else
        {
            // Handle unauthorized access for non-student users.
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpPost]
    public ActionResult Upload(HttpPostedFileBase file, string degree, string board, double percentage, string yearOfPassing = "0")
    {
        if (Session["UserRole"]?.ToString() == "Student" && file != null && file.ContentLength > 0)
        {
            byte[] fileData;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            var pdfFile = new PdfFile
            {
                FileName = file.FileName,
                FileData = fileData,
                Degree = degree,
                Board = board,
                Percentage = percentage,
                Year_of_Passing = yearOfPassing.ToString() // Convert yearOfPassing to string
            };

            int userId = Convert.ToInt32(Session["UserId"]);

            if (UploadPDFFile(pdfFile.FileName, fileData, userId, degree, board, percentage, yearOfPassing))
            {
                return RedirectToAction("PdfList");
            }
            else
            {
                ModelState.AddModelError("", "Failed to upload the PDF file.");
            }
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    private bool UploadPDFFile(string fileName, byte[] fileData, int userId, string degree, string board, double percentage, string yearOfPassing = "0")
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("UploadPDFFile", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@FileData", fileData);
                    command.Parameters.AddWithValue("@User_ID", userId);
                    command.Parameters.AddWithValue("@Degree", degree); // Add the 'Degree' parameter
                    command.Parameters.AddWithValue("@Board", board); // Add the 'Board' parameter
                    command.Parameters.AddWithValue("@Percentage", percentage); // Add the 'Percentage' parameter
                    command.Parameters.AddWithValue("@Year_of_Passing", yearOfPassing); // Add the 'Year_of_Passing' parameter

                    command.ExecuteNonQuery();

                    return true; // Successful upload
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately
            return false; // Upload failed
        }
    }


    public ActionResult PdfList()
    {
        // Fetch the list of PDF files from your database or repository
        List<PdfFile> pdfFiles = GetPDFFilesFromDatabase();

        // Pass the list of PDF files to the view
        return View(pdfFiles);
    }

    private List<PdfFile> GetPDFFilesFromDatabase()
    {
        // Implement logic to fetch PDF files from your database or repository
        // Example:
        List<PdfFile> pdfFiles = new List<PdfFile>();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT * FROM PDFFile", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PdfFile pdfFile = new PdfFile
                            {
                                File_ID = (int)reader["File_ID"],
                                FileName = reader["FileName"].ToString(),
                                FileData = (byte[])reader["FileData"],
                                Degree = reader["Degree"].ToString(),
                                Board = reader["Board"].ToString(),
                                Percentage = reader.IsDBNull(reader.GetOrdinal("Percentage")) ? 0.0 : (double)reader["Percentage"],
                                Year_of_Passing = reader["Year_of_Passing"].ToString()


                            };
                            pdfFiles.Add(pdfFile);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately
        }

        return pdfFiles;
    }

    public ActionResult ViewPdf(int id)
    {
        // Retrieve the PDF file by its ID from the database
        PdfFile pdfFile = GetPdfFileById(id);

        if (pdfFile != null)
        {
            // Convert the byte array to a stream
            var stream = new MemoryStream(pdfFile.FileData);

            // Return the PDF file as a FileResult
            return File(stream, "application/pdf");
        }
        else
        {
            // Handle the case where the PDF file is not found
            return RedirectToAction("NotFound", "Error"); // You can create an error handling action for this
        }
    }

    public ActionResult DownloadPdf(int id)
    {
        // Retrieve the PDF file by its ID from the database
        PdfFile pdfFile = GetPdfFileById(id);

        if (pdfFile != null)
        {
            // Convert the byte array to a stream and return it as a File result
            var stream = new MemoryStream(pdfFile.FileData);
            return File(stream, "application/pdf", pdfFile.FileName);
        }
        else
        {
            // Handle the case where the PDF file is not found
            return RedirectToAction("NotFound", "Error"); // You can create an error handling action for this
        }
    }

    private PdfFile GetPdfFileById(int id)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct the SQL query to retrieve the PDF file by ID
                string sql = "SELECT File_ID, FileName, FileData, Degree, Board FROM PDFFile WHERE File_ID = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PdfFile
                            {
                                File_ID = (int)reader["File_ID"],
                                FileName = reader["FileName"].ToString(),
                                FileData = (byte[])reader["FileData"],
                                Degree = reader["Degree"].ToString(),
                                Board = reader["Board"].ToString(),
                                Percentage = (double)reader["Percentage"],
                                Year_of_Passing = reader["Year_of_Passing"].ToString()
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately
        }

        return null; // Return null if the PDF file is not found or an error occurs
    }



    /*================================================*/


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


    private bool UploadNonAcademicInfo(int year, string category, string activity, string certificatePath, int userId)
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
                                Year = (int)reader["Year"],
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
