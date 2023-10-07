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
    public ActionResult Upload(HttpPostedFileBase file)
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
                FileData = fileData
            };

            
            int userId = Convert.ToInt32(Session["UserId"]); // Updated session variable name

            // Call the UploadPDFFile stored procedure to upload the PDF file.
            if (UploadPDFFile(pdfFile.FileName, pdfFile.FileData, userId))
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
            // Handle unauthorized access or invalid file upload for non-student users.
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    private bool UploadPDFFile(string fileName, byte[] fileData, int userId)
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
                                FileData = (byte[])reader["FileData"]
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
                string sql = "SELECT File_ID, FileName, FileData, User_ID FROM PDFFile WHERE File_ID = @Id";

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
                                User_ID = (int)reader["User_ID"]
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



}
