using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIMCMD.Data;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCMD.Controllers
{
    [Authorize(Roles = "Root, Admin, Accounts")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportController> _logger;

        public ReportController(ApplicationDbContext context, ILogger<ReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ReportRequest
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation($"{User.Identity.Name} accessed report list page");

            return View(await _context.ReportRequest.ToListAsync());
        }

        // GET: ReportRequest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReportRequest == null)
            {
                return NotFound();
            }

            var reportRequest = await _context.ReportRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportRequest == null)
            {
                return NotFound();
            }

            return View(reportRequest);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var reportRequest = new ReportRequest
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                ReportData = "<p>No report generated yet.</p>"
            };

            return View(reportRequest);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(ReportRequest reportRequest, string actionType)
        {
            if (actionType == "search")
            {
                return View(reportRequest);
            }
            else if (actionType == "create" && ModelState.IsValid)
            {
                try
                {
                    // Filtering records within the specified date range and grouping by Provider.
                    var reportData = _context.FileConversion
                        .Where(fc => fc.DateCreated >= reportRequest.StartDate && fc.DateCreated <= reportRequest.EndDate)
                        .GroupBy(fc => fc.Provider)
                        .Select(g => new 
                        {
                            Provider = g.Key,
                            UploadedCount = g.Count(x => x.IsFileUploaded),
                            UnwrappedCount = g.Count(x => x.IsFileUnwraped),
                            ConvertedCount = g.Count(x => x.IsFileConverted),
                            ImportedCount = g.Count(x => x.IsFileImported)
                        }).ToList();

                    // Generating HTML string for displaying the report.
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append("<h2>Provider Report</h2>");
                    stringBuilder.Append("<table class='table'><thead><tr><th>Provider</th><th>Uploaded</th><th>Unwrapped</th><th>Converted</th><th>Imported</th></tr></thead><tbody>");

                    foreach (var item in reportData)
                    {
                        stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", 
                            item.Provider, item.UploadedCount, item.UnwrappedCount, item.ConvertedCount, item.ImportedCount);
                    }

                    stringBuilder.Append("</tbody></table>");
                    reportRequest.ReportData = stringBuilder.ToString();

                    // Assuming you want to save the report request for record keeping.
                    _context.Add(reportRequest);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred generating provider report: {ErrorMessage}", ex.Message);
                    return RedirectToAction("ErrorPage", "Home");
                }
            }

            return View(reportRequest);
        }
    }
}
