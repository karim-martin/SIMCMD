using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIMCMD.Models;
using SIMCMD.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SIMCMD.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityExtendUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityExtendUser> userManager, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public IActionResult ErrorPage()
        {
            return View();
        }

        [Authorize(Roles = "Root, Admin, Claim")]
        public IActionResult Index()
        {
            var provider = new Provider();
            var manualFileUpload = new ManualFileUpload();
            var fileConversion = new FileConversion();
            
            ViewBag.IsFileUploadedTable = _context.FileConversion.Where(s => s.IsFileUploaded == true).Count();
            ViewBag.IsFileConvertedTable = _context.FileConversion.Where(s => s.IsFileConverted == true).Count();
            ViewBag.IsFileImportedTable = _context.FileConversion.Where(s => s.IsFileImported == true).Count();
            
            return View();
        }

        public IActionResult TotalClaimsChart()
        {
            var currentYear = DateTime.Now.Year;
            var monthlyClaims = _context.FileConversion
                .Where(data => data.DateCreated.HasValue && data.DateCreated.Value.Year == currentYear) // Ensure DateCreated has a value and matches the current year
                .GroupBy(data => new { data.DateCreated.Value.Year, data.DateCreated.Value.Month }) // Group by year and month
                .Select(group => new
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1), // Use DateTime to represent the month
                    TotalClaims = group.Sum(data => data.TotalClaims) // Sum TotalClaims for the group
                })
                .ToList();

            var chartData = new decimal[12]; // Initialize with 0 for each month
            var monthLabels = Enumerable.Range(1, 12)
                .Select(i => new DateTime(currentYear, i, 1).ToString("yyyy-MM")) // Generate labels in "yyyy-MM" format
                .ToList();

            foreach (var claimRecord in monthlyClaims)
            {
                var period = claimRecord.Month.ToString("yyyy-MM"); // Convert to "yyyy-MM" format
                int monthIndex = monthLabels.IndexOf(period); // Find the index based on the period

                if (monthIndex >= 0)
                {
                    chartData[monthIndex] = claimRecord.TotalClaims; // Assign TotalClaims to the corresponding month
                }
            }

            return Json(new { labels = monthLabels, data = chartData });
        }

        public IActionResult TotalClaimsCountChart()
        {
            var currentYear = DateTime.Now.Year;
            var monthlyFileConversions = _context.FileConversion
                .Where(data => data.DateCreated.HasValue && data.DateCreated.Value.Year == currentYear && data.IsFileUploaded) // Ensure DateCreated has a value, for the current year, and file is converted
                .GroupBy(data => new { data.DateCreated.Value.Year, data.DateCreated.Value.Month }) // Group by year and month
                .Select(group => new
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1), // Use DateTime to represent the month
                    TotalFileUploaded = group.Count() // Count the number of files converted in each group
                })
                .ToList();

            var chartData = new int[12]; // Initialize with 0 for each month
            var monthLabels = Enumerable.Range(1, 12)
                .Select(i => new DateTime(currentYear, i, 1).ToString("yyyy-MM")) // Generate labels in "yyyy-MM" format
                .ToList();

            foreach (var conversionRecord in monthlyFileConversions)
            {
                var period = conversionRecord.Month.ToString("yyyy-MM"); // Convert to "yyyy-MM" format
                int monthIndex = monthLabels.IndexOf(period); // Find the index based on the period

                if (monthIndex >= 0)
                {
                    chartData[monthIndex] = conversionRecord.TotalFileUploaded; // Assign the count of files converted to the corresponding month
                }
            }

            return Json(new { labels = monthLabels, data = chartData });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
