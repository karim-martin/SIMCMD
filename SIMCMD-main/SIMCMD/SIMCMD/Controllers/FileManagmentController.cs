using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIMCMD.Data;
using System.Threading.Tasks;
using SIMCMD.Models;
using System.Collections.Generic;
using ExcelDataReader;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Data;
using System;

namespace SIMCMD.Controllers
{
    [Authorize(Roles = "Root, Admin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // GET: DocumentController
        public IActionResult FileManagment(string folderName)
        {
            string path;
            path = _configuration.GetValue<string>($"FolderPaths:{folderName}");
            if (string.IsNullOrEmpty(path))
            {
                // Handle the case where the folderName does not match any key in appsettings.json
                return NotFound($"The specified folder '{folderName}' is not configured.");
            }

            // Get a list of all the files in the folder
            var fileNames = Directory.GetFileSystemEntries(path);

            // Get information about each file
            var files = new List<FileManagement>();
            foreach (var fileName in fileNames)
            {
                // Get the full path of the file
                var filePath = Path.Combine(path, fileName);

                if (Directory.Exists(filePath))
                {
                    // If the file is a directory, add it as a subfolder
                    files.Add(new FileManagement
                    {
                        FileName = Path.GetFileName(filePath),
                        IsDirectory = true,
                        FilePath = filePath
                    });
                }
                else
                {
                    // If the file is a regular file, add it to the list
                    var fileInfo = new FileInfo(filePath);

                    files.Add(new FileManagement
                    {
                        FileName = fileInfo.Name,
                        LastModified = fileInfo.LastWriteTime,
                        FileSize = fileInfo.Length,
                        FilePath = filePath
                    });
                }
            }

            return View(files);
        }

        public IActionResult Download(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var fileExtension = Path.GetExtension(path).ToLowerInvariant();
                var contentType = fileExtension switch
                {
                    ".txt" => "text/plain",
                    ".csv" => "text/csv",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".837i" => "text/plain",
                    ".x12" => "text/plain",
                    _ => "application/octet-stream",
                };

                var fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, contentType, Path.GetFileName(path));
            }
            else
            {
                return NotFound("File not found.");
            }
        }


        public IActionResult ViewTextFile(string path)
        {
            var fileExtension = Path.GetExtension(path).ToLowerInvariant();
            var supportedTextTypes = new HashSet<string> { ".txt", ".csv", ".837i", ".x12" };
            var supportedExcelTypes = new HashSet<string> { ".xls", ".xlsx" };

            if (!System.IO.File.Exists(path))
            {
                return NotFound("File not found.");
            }

            if (supportedTextTypes.Contains(fileExtension))
            {
                var fileContent = System.IO.File.ReadAllText(path);
                return Content(fileContent, "text/plain");
            }
            else if (supportedExcelTypes.Contains(fileExtension))
            {
                try
                {
                    using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet();
                            var stringBuilder = new StringBuilder();

                            // Assuming you want to display the first sheet only for simplicity
                            if (result.Tables.Count > 0)
                            {
                                var table = result.Tables[0];
                                foreach (DataRow row in table.Rows)
                                {
                                    foreach (var item in row.ItemArray)
                                    {
                                        stringBuilder.Append(item.ToString() + "\t");
                                    }
                                    stringBuilder.AppendLine();
                                }
                            }

                            return Content(stringBuilder.ToString(), "text/plain");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (ex) if logging is set up
                    return Content($"Error reading Excel file: {ex.Message}");
                }
            }
            else
            {
                return Content("Unsupported file format for viewing.");
            }
        }
    }
}